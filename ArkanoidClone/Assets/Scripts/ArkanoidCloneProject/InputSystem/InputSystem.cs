using System;
using ArkanoidCloneProject.InputSystem;
using UnityEngine.InputSystem;

public sealed class InputManager : IInputManager, IDisposable
{
    private readonly GameControls actions;

    public event Action OnSpaceButtonDown;
    public event Action OnSpaceButtonUp;

    public event Action OnLeftButtonDown;
    public event Action OnLeftButtonUp;

    public event Action OnRightButtonDown;
    public event Action OnRightButtonUp;

    public InputManager()
    {
        actions = new GameControls();

        Bind(actions.Player.Space,
            () => OnSpaceButtonDown?.Invoke(),
            () => OnSpaceButtonUp?.Invoke());

        Bind(actions.Player.MoveLeft,
            () => OnLeftButtonDown?.Invoke(),
            () => OnLeftButtonUp?.Invoke());

        Bind(actions.Player.MoveRight,
            () => OnRightButtonDown?.Invoke(),
            () => OnRightButtonUp?.Invoke());

        actions.Enable();
    }

    private static void Bind(
        InputAction action,
        Action onDown,
        Action onUp)
    {
        action.started += _ => onDown?.Invoke();
        action.canceled += _ => onUp?.Invoke();
    }

    public void Dispose()
    {
        actions.Disable();
        actions.Dispose();
    }
}