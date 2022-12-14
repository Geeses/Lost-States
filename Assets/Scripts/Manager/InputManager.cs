using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : NetworkBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;
    private static InputManager s_instance;
    private Camera _cam;
    private CameraController _cameraController;

    [SerializeField] private Selectable _selectedObject;

    public static InputManager Instance { get { return s_instance; } }
    public PlayerInput PlayerInput { get => _playerInput; set => _playerInput = value; }
    public PlayerInputActions PlayerInputActions { get => _playerInputActions; set => _playerInputActions = value; }

    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }

        // Input Init
        PlayerInput = GetComponent<PlayerInput>();

        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Player.Enable();
    }

    private void Start()
    {
        _cam = Camera.main;
        _cameraController = _cam.GetComponent<CameraController>();

        PlayerInputActions.Player.Select.performed += Select;
        PlayerInputActions.Player.Move.performed += Move;
    }

    private void OnDisable()
    {
        PlayerInputActions.Player.Select.performed -= Select;
        PlayerInputActions.Player.Move.performed -= Move;
    }

    private void Select(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(_cam.ScreenToWorldPoint(new Vector3(_cameraController.MousePosition.x, _cameraController.MousePosition.y, -_cam.transform.position.z)), Vector2.zero);

        if (hit.collider != null)
        {
            Transform objectHit = hit.transform;

            if (objectHit.CompareTag("Selectable") || objectHit.CompareTag("Player"))
            {
                Selectable selectable = objectHit.GetComponent<Selectable>();

                selectable.Select();

                if (_selectedObject != null && _selectedObject != selectable)
                {
                    _selectedObject.Unselect();
                }

                _selectedObject = selectable;
            }
        }
    }

    private void Move(InputAction.CallbackContext obj)
    {
        RaycastHit2D hit = Physics2D.Raycast(_cam.ScreenToWorldPoint(new Vector3(_cameraController.MousePosition.x, _cameraController.MousePosition.y, -_cam.transform.position.z)), Vector2.zero);

        if (hit.collider != null)
        {
            Selectable objectHit = hit.transform.GetComponent<Selectable>();

            if (objectHit.CompareTag("Selectable") && _selectedObject != null && _selectedObject.CompareTag("Player") && (TurnManager.Instance.CurrentTurnPlayerId == NetworkManager.LocalClientId))
            {
                // get cellposition from tilemap, convert it to GridCoordinates and move client to the grid position
                Vector3Int cellPosition = GridManager.Instance.Tilemap.LocalToCell(objectHit.transform.position);
                GridCoordinates coordinates = new GridCoordinates(cellPosition.x, cellPosition.y);
                _selectedObject.GetComponent<Player>().TryMoveServerRpc(coordinates);
            }
        }
    }
}
