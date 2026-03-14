using Sirenix.OdinInspector;
using UnityEngine;

namespace GravityThread.Configs
{
    [CreateAssetMenu(fileName = "ThreadConfig", menuName = "GravityThread/Thread Config")]
    public sealed class ThreadConfig : SerializedScriptableObject
    {
        [Title("Energy")]
        [Range(1f, 100f)] public float MaxEnergy = 100f;
        [Range(0.5f, 20f)] public float DrainRate = 10f;
        [Range(0.5f, 15f)] public float RegenRate = 5f;
        [Range(0f, 2f)] public float RegenDelay = 0.5f;

        [Title("Force")]
        [Range(1f, 50f)] public float PullForce = 15f;
        [Range(1f, 20f)] public float MaxDistance = 10f;
        [Range(0f, 1f)] public float DistanceFalloff = 0.3f;
    }
}
