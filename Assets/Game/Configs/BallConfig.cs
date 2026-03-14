using Sirenix.OdinInspector;
using UnityEngine;

namespace GravityThread.Configs
{
    [CreateAssetMenu(fileName = "BallConfig", menuName = "GravityThread/Ball Config")]
    public sealed class BallConfig : SerializedScriptableObject
    {
        [Title("Physics")]
        [Range(0.1f, 10f)] public float Mass = 1f;
        [Range(0.5f, 30f)] public float MaxSpeed = 15f;
        [Range(0f, 1f)] public float Bounciness = 0.4f;
        [Range(0f, 1f)] public float Friction = 0.3f;

        [Title("Health")]
        [Range(1, 10)] public int MaxHealth = 3;
        [Range(0f, 5f)] public float WallDamageForceThreshold = 3f;

        [Title("Size")]
        [Range(0.1f, 2f)] public float DefaultScale = 0.5f;
        [Range(0.1f, 0.5f)] public float MinScale = 0.2f;
        [Range(0.5f, 3f)] public float MaxScale = 1.5f;
        [Range(0.01f, 0.5f)] public float SizeChangePerBottle = 0.1f;
        [Range(0.01f, 0.5f)] public float SizeChangePerSpikeHit = 0.15f;

        [Title("Trail")]
        public Gradient TrailGradient;
        [Range(0.05f, 2f)] public float BaseTrailWidth = 0.15f;
        [Range(0.1f, 3f)] public float TrailWidthMultiplierAtMaxSpeed = 2.5f;
        [Range(0.1f, 2f)] public float TrailLifetime = 0.5f;
    }
}
