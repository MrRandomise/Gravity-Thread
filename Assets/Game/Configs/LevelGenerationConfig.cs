using Sirenix.OdinInspector;
using UnityEngine;

namespace GravityThread.Configs
{
    [CreateAssetMenu(fileName = "LevelGenConfig", menuName = "GravityThread/Level Generation Config")]
    public sealed class LevelGenerationConfig : SerializedScriptableObject
    {
        [Title("Grid")]
        [Range(4, 20)] public int GridWidth = 8;
        [Range(8, 40)] public int GridHeight = 16;
        [Range(0.5f, 3f)] public float CellSize = 1f;

        [Title("Path")]
        [Range(1, 6)] public int PathWidth = 2;
        [Range(2, 10)] public int MinStraightLength = 3;
        [Range(3, 15)] public int MaxStraightLength = 7;

        [Title("Obstacles")]
        [Range(0f, 1f)] public float SpikeChance = 0.15f;
        [Range(0f, 1f)] public float ColorGateChance = 0.08f;
        [Range(0f, 1f)] public float PulsingWallChance = 0.1f;
        [Range(0f, 1f)] public float BottleChance = 0.12f;

        [Title("Difficulty Scaling")]
        [Range(0f, 0.1f)] public float SpikeChancePerLevel = 0.01f;
        [Range(0f, 0.05f)] public float NarrowingPerLevel = 0.005f;

        [Title("Prefabs")]
        public GameObject WallPrefab;
        public GameObject SpikePrefab;
        public GameObject ColorGatePrefab;
        public GameObject PulsingWallPrefab;
        public GameObject BottlePrefab;
        public GameObject GoalPrefab;
        public GameObject ColorSourcePrefab;
    }
}
