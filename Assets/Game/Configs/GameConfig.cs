using Sirenix.OdinInspector;
using UnityEngine;

namespace GravityThread.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GravityThread/Game Config")]
    public sealed class GameConfig : SerializedScriptableObject
    {
        [Title("Scoring")]
        [Range(100, 10000)] public int BaseScorePerLevel = 1000;
        [Range(1f, 5f)] public float NoCollisionMultiplier = 2f;
        [Range(1f, 3f)] public float LowEnergyBonusMultiplier = 1.5f;
        [Range(0f, 0.2f)] public float LowEnergyThreshold = 0.2f;

        [Title("Timing")]
        [Range(10f, 120f)] public float LevelDuration = 60f;

        [Title("Visuals")]
        public Color DefaultBallColor = Color.white;
        public Color[] AvailableColors = { Color.green, Color.cyan, Color.magenta };
    }
}
