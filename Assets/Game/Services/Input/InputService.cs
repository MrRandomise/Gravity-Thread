using UnityEngine;
using UnityEngine.InputSystem;
using GravityThread.Core.Interfaces;

namespace GravityThread.Services.Input
{
    public sealed class InputService : IInputService, IGameTickable
    {
        private Camera _camera;
        private readonly InputAction _pressAction;
        private readonly InputAction _positionAction;

        public bool IsTouching { get; private set; }
        public Vector2 TouchWorldPosition { get; private set; }

        public InputService(Camera camera)
        {
            _camera = camera;

            _pressAction = new InputAction("Press", InputActionType.Button);
            _pressAction.AddBinding("<Mouse>/leftButton");
            _pressAction.AddBinding("<Touchscreen>/primaryTouch/press");

            _positionAction = new InputAction("Position", InputActionType.Value, binding: "<Pointer>/position");

            _pressAction.Enable();
            _positionAction.Enable();
        }

        public void GameTick(float deltaTime)
        {
            if (_camera == null)
                _camera = Camera.main;

            IsTouching = _pressAction.IsPressed();

            if (IsTouching && _camera != null)
            {
                Vector2 screenPos = _positionAction.ReadValue<Vector2>();
                TouchWorldPosition = _camera.ScreenToWorldPoint(screenPos);
            }
        }

        ~InputService()
        {
            _pressAction?.Disable();
            _pressAction?.Dispose();
            _positionAction?.Disable();
            _positionAction?.Dispose();
        }
    }
}