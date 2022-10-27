using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;
    private static InputManager s_instance;

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
}
