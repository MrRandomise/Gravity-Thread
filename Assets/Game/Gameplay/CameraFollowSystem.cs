using GravityThread.Core.Interfaces;
using UnityEngine;

namespace GravityThread.Gameplay
{
    public sealed class CameraFollowSystem : IGameLateTickable
    {
        private readonly BallController _ball;
        private Transform _cameraTransform;
        private float _smoothSpeed = 5f;
        private float _offsetZ = -10f;

        public CameraFollowSystem(BallController ball)
        {
            _ball = ball;
        }

        public void Initialize(Transform cameraTransform, float smoothSpeed = 5f)
        {
            _cameraTransform = cameraTransform;
            _smoothSpeed = smoothSpeed;
        }

        public void GameLateTick(float deltaTime)
        {
            if (_cameraTransform == null) return;

            Vector3 targetPos = new Vector3(_ball.Position.x, _ball.Position.y, _offsetZ);
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPos, _smoothSpeed * deltaTime);
        }
    }
}
