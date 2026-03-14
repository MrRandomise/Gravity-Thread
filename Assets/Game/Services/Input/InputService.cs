using UnityEngine;
using GravityThread.Core.Interfaces;

namespace GravityThread.Services.Input
{
    public sealed class InputService : IInputService, IGameTickable
    {
        private Camera _camera;

        public bool IsTouching { get; private set; }
        public Vector2 TouchWorldPosition { get; private set; }

        public InputService(Camera camera)
        {
            _camera = camera;
        }

        public void GameTick(float deltaTime)
        {
            if (_camera == null)
                _camera = Camera.main;

#if UNITY_EDITOR || UNITY_STANDALONE
            IsTouching = UnityEngine.Input.GetMouseButton(0);
            if (IsTouching && _camera != null)
                TouchWorldPosition = _camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
#else
            if (UnityEngine.Input.touchCount > 0)
            {
                IsTouching = true;
                if (_camera != null)
                    TouchWorldPosition = _camera.ScreenToWorldPoint(UnityEngine.Input.GetTouch(0).position);
            }
            else
            {
                IsTouching = false;
            }
#endif
        }
    }
}
