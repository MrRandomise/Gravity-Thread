using GravityThread.Core.Interfaces;
using UnityEngine;

namespace GravityThread.Gameplay
{
    public sealed class TrailMaterialController : IGameLateTickable
    {
        private readonly BallController _ball;
        private Material _trailMaterial;

        private static readonly int SpeedFactorId = Shader.PropertyToID("_SpeedFactor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        public TrailMaterialController(BallController ball)
        {
            _ball = ball;
        }

        public void SetMaterial(Material material)
        {
            _trailMaterial = material;
        }

        public void GameLateTick(float deltaTime)
        {
            if (_trailMaterial == null) return;

            float speedRatio = Mathf.Clamp01(_ball.Speed / 15f);
            _trailMaterial.SetFloat(SpeedFactorId, speedRatio);
            _trailMaterial.SetColor(ColorId, _ball.CurrentColor);
        }
    }
}
