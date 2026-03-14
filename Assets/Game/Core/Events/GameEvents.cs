using UnityEngine;

namespace GravityThread.Core.Events
{
    // --- Thread / Energy ---
    public struct ThreadEnergyChangedEvent
    {
        public float Current;
        public float Max;
        public float Normalized => Max > 0f ? Current / Max : 0f;
    }

    // --- Ball ---
    public struct BallHealthChangedEvent
    {
        public int Current;
        public int Max;
    }

    public struct BallSizeChangedEvent
    {
        public float NewScale;
    }

    public struct BallColorChangedEvent
    {
        public Color NewColor;
    }

    public struct BallDestroyedEvent { }

    public struct BallWallCollisionEvent
    {
        public float ImpactForce;
    }

    // --- Score ---
    public struct ScoreChangedEvent
    {
        public int Score;
    }

    // --- Level ---
    public struct LevelStartedEvent
    {
        public int LevelIndex;
    }

    public struct LevelCompletedEvent
    {
        public int LevelIndex;
        public int Score;
        public bool NoCollisions;
        public float EnergyUsedPercent;
    }

    public struct LevelFailedEvent
    {
        public int LevelIndex;
    }

    // --- Game State ---
    public struct GamePausedEvent
    {
        public bool IsPaused;
    }

    // --- Collectibles ---
    public struct BottleCollectedEvent
    {
        public Vector3 Position;
    }

    // --- Obstacle ---
    public struct SpikeHitEvent
    {
        public Vector3 Position;
    }

    public struct ColorGateReachedEvent
    {
        public Color RequiredColor;
    }
}
