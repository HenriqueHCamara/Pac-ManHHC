using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    public static event Action<Vector2> onMovementInput;
    public void onMoveInput(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();

        if (context.started)
        {
            if (movementInput.Equals(Vector2.up))
                onMovementInput?.Invoke(Vector2.up);

            else if (movementInput.Equals(Vector2.down))
                onMovementInput?.Invoke(Vector2.down);

            else if (movementInput.Equals(Vector2.left))
                onMovementInput?.Invoke(Vector2.left);

            else if (movementInput.Equals(Vector2.right))
                onMovementInput?.Invoke(Vector2.right);
        }
    }
}
