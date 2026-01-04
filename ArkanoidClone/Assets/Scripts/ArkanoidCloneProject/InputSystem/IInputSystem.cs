using System;

namespace ArkanoidCloneProject.InputSystem
{
    public interface IInputManager
    {
        event Action OnSpaceButtonDown;
        event Action OnSpaceButtonUp;

        event Action OnLeftButtonDown;
        event Action OnLeftButtonUp;

        event Action OnRightButtonDown;
        event Action OnRightButtonUp;
    }
}