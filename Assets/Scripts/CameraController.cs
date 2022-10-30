using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject map;

    [Header("Options")]
    public float panSpeed = 1f;
    public Vector2 panningZone = new Vector2(5, 18);
    public CursorLockMode cursorMode = CursorLockMode.Confined;
    public float minZoomValue = 0.5f;
    public float maxZoomValue = 1.5f;
    public float zoomSpeed = 1f;
    public float fieldOfView = 70;

    private float zoomValue = 70f;
    private Vector2 _mousePositionScreenRelative;
    private Vector2 _mousePosition;

    private const int UnityScrollValue = 120;

    [SerializeField] private List<Selectable> _selectedObjects = new List<Selectable>();

    public Vector2 MousePositionScreenRelative { get => _mousePositionScreenRelative; set => _mousePositionScreenRelative = value; }
    public Vector2 MousePosition { get => _mousePosition; set => _mousePosition = value; }

    void Start()
    {
        Camera.main.fieldOfView = fieldOfView;
        Cursor.lockState = cursorMode;
        InputManager.Instance.PlayerInputActions.Player.Select.performed += Select;
        InputManager.Instance.PlayerInputActions.Player.Look.performed += UpdateMousePosition;
        InputManager.Instance.PlayerInputActions.Player.Zoom.performed += Zoom;
    }

    private void Zoom(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<Vector2>().y;

        if ((value < 0 && fieldOfView * maxZoomValue > zoomValue) || (value > 0 && fieldOfView * minZoomValue < zoomValue))
        {
            // unity ready for each wheel scroll +120 for up or -120 for down so we normalize it
            zoomValue -= value / (UnityScrollValue * zoomSpeed);
            Camera.main.fieldOfView = zoomValue;
        }
    }

    private void UpdateMousePosition(InputAction.CallbackContext ctx)
    {
        MousePosition = ctx.ReadValue<Vector2>();
        MousePositionScreenRelative = new Vector2(MousePosition.x / Screen.width, MousePosition.y / Screen.height);
    }

    private void FixedUpdate()
    {
        if(MousePositionScreenRelative.x >= 0.95f && Camera.main.transform.position.x <= panningZone.x)
        {
            PanCamera(Vector2.right);
        }
        if (MousePositionScreenRelative.x <= 0.05f && Camera.main.transform.position.x >= -panningZone.x)
        {
            PanCamera(Vector2.left);
        }

        if(MousePositionScreenRelative.y >= 0.95f && Camera.main.transform.position.y <= panningZone.y)
        {
            PanCamera(Vector2.up);
        }
        if (MousePositionScreenRelative.y <= 0.05f && Camera.main.transform.position.y >= -panningZone.y)
        {
            PanCamera(Vector2.down);
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.PlayerInputActions.Player.Select.performed -= Select;
        InputManager.Instance.PlayerInputActions.Player.Look.performed -= UpdateMousePosition;
    }

    private void Select(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, -Camera.main.transform.position.z)), Vector2.zero);

        if (hit.collider != null)
        {
            Transform objectHit = hit.transform;

            if (objectHit.tag == "Selectable")
            {
                Selectable selectable = objectHit.GetComponent<Selectable>();

                if (!selectable.Selected)
                {
                    foreach (var unit in _selectedObjects)
                    {
                        unit.Unselect();
                    }

                    _selectedObjects.Clear();
                    _selectedObjects.Add(selectable);
                    selectable.Select();
                }
            }
            else
            {
                if (_selectedObjects.Count > 0)
                {
                    foreach (var selectable in _selectedObjects)
                    {
                        selectable.Unselect();
                    }

                    _selectedObjects.Clear();
                }
            }
        }
    }

    private void PanCamera(Vector2 direction)
    {
        gameObject.transform.Translate(new Vector3(direction.x * panSpeed, direction.y * panSpeed, 0f), Space.Self);
    }
}
