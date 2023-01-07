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
    public Vector2 panningZoneStartPoint = new Vector2(0, 0);
    public CursorLockMode cursorMode = CursorLockMode.Confined;
    public float minZoomValue = 0.5f;
    public float maxZoomValue = 1.5f;
    public float zoomSpeed = 1f;
    public float fieldOfView = 70;

    private float _zoomValue;
    private Vector2 _mousePositionScreenRelative;
    private Vector2 _mousePosition;
    private Camera cam;

    private const int UnityScrollValue = 120;


    public Vector2 MousePositionScreenRelative { get => _mousePositionScreenRelative; set => _mousePositionScreenRelative = value; }
    public Vector2 MousePosition { get => _mousePosition; set => _mousePosition = value; }
    public Camera Camera { get => cam; set => cam = value; }

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = fieldOfView;
        Cursor.lockState = cursorMode;
        _zoomValue = fieldOfView;

        InputManager.Instance.PlayerInputActions.Player.Look.performed += UpdateMousePosition;
        InputManager.Instance.PlayerInputActions.Player.Zoom.performed += Zoom;
    }

    private void FixedUpdate()
    {
        if (MousePositionScreenRelative.x >= 0.95f && cam.transform.position.x <= panningZone.x + panningZoneStartPoint.x)
        {
            PanCamera(Vector2.right);
        }
        if (MousePositionScreenRelative.x <= 0.05f && cam.transform.position.x >= -panningZone.x + panningZoneStartPoint.x)
        {
            PanCamera(Vector2.left);
        }

        if (MousePositionScreenRelative.y >= 0.95f && cam.transform.position.y <= panningZone.y + panningZoneStartPoint.y)
        {
            PanCamera(Vector2.up);
        }
        if (MousePositionScreenRelative.y <= 0.05f && cam.transform.position.y >= -panningZone.y + panningZoneStartPoint.y)
        {
            PanCamera(Vector2.down);
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.PlayerInputActions.Player.Look.performed -= UpdateMousePosition;
    }

    private void Zoom(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<Vector2>().y;

        if ((value < 0 && fieldOfView * maxZoomValue > _zoomValue) || (value > 0 && fieldOfView * minZoomValue < _zoomValue))
        {
            // unity ready for each wheel scroll +120 for up or -120 for down so we normalize it
            _zoomValue -= value / (UnityScrollValue * zoomSpeed);
            cam.fieldOfView = _zoomValue;
        }
    }

    private void UpdateMousePosition(InputAction.CallbackContext ctx)
    {
        MousePosition = ctx.ReadValue<Vector2>();
        MousePositionScreenRelative = new Vector2(MousePosition.x / Screen.width, MousePosition.y / Screen.height);
    }

    private void PanCamera(Vector2 direction)
    {
        gameObject.transform.Translate(new Vector3(direction.x * panSpeed, direction.y * panSpeed, 0f), Space.Self);
    }
}
