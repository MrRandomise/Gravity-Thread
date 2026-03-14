using System.Collections.Generic;
using GravityThread.Core.Events;
using GravityThread.Services.Debug;

namespace GravityThread.Services.Achievements
{
    public enum AchievementType
    {
        PerfectLevel,      // No wall collisions
        EnergyMaster,      // Used less than 20% energy
        Unstoppable,       // 5 levels in a row without dying
        Collector          // Collected 50 bottles total
    }

    public sealed class AchievementData
    {
        public AchievementType Type;
        public string NameKey;
        public string DescKey;
        public bool IsUnlocked;
    }

    public interface IAchievementService
    {
        IReadOnlyList<AchievementData> Achievements { get; }
        void CheckLevelCompletion(LevelCompletedEvent e);
    }

    public sealed class AchievementService : IAchievementService
    {
        private readonly List<AchievementData> _achievements;
        private readonly EventBus _eventBus;
        private readonly IDebugService _debug;

        private int _consecutiveLevels;
        private int _totalBottles;

        public IReadOnlyList<AchievementData> Achievements => _achievements;

        public AchievementService(EventBus eventBus, IDebugService debug)
        {
            _eventBus = eventBus;
            _debug = debug;

            _achievements = new List<AchievementData>
            {
                new AchievementData { Type = AchievementType.PerfectLevel, NameKey = "ach.perfect.name", DescKey = "ach.perfect.desc" },
                new AchievementData { Type = AchievementType.EnergyMaster, NameKey = "ach.energy.name", DescKey = "ach.energy.desc" },
                new AchievementData { Type = AchievementType.Unstoppable, NameKey = "ach.unstoppable.name", DescKey = "ach.unstoppable.desc" },
                new AchievementData { Type = AchievementType.Collector, NameKey = "ach.collector.name", DescKey = "ach.collector.desc" },
            };

            _eventBus.Subscribe<BottleCollectedEvent>(OnBottleCollected);
        }

        public void CheckLevelCompletion(LevelCompletedEvent e)
        {
            if (e.NoCollisions)
                Unlock(AchievementType.PerfectLevel);

            if (e.EnergyUsedPercent < 0.2f)
                Unlock(AchievementType.EnergyMaster);

            _consecutiveLevels++;
            if (_consecutiveLevels >= 5)
                Unlock(AchievementType.Unstoppable);
        }

        public void OnLevelFailed()
        {
            _consecutiveLevels = 0;
        }

        private void OnBottleCollected(BottleCollectedEvent e)
        {
            _totalBottles++;
            if (_totalBottles >= 50)
                Unlock(AchievementType.Collector);
        }

        private void Unlock(AchievementType type)
        {
            foreach (var ach in _achievements)
            {
                if (ach.Type == type && !ach.IsUnlocked)
                {
                    ach.IsUnlocked = true;
                    _debug.Log($"Achievement unlocked: {ach.NameKey}");
                }
            }
        }
    }
}
