using GravityThread.Configs;
using GravityThread.Core.Events;

namespace GravityThread.Services.Score
{
    public interface IScoreService
    {
        int CurrentScore { get; }
        int TotalScore { get; }
        void AddScore(int amount);
        void ResetLevelScore();
        int CalculateLevelScore(bool noCollisions, float energyUsedPercent);
    }

    public sealed class ScoreService : IScoreService
    {
        private readonly GameConfig _config;
        private readonly EventBus _eventBus;

        public int CurrentScore { get; private set; }
        public int TotalScore { get; private set; }

        public ScoreService(GameConfig config, EventBus eventBus)
        {
            _config = config;
            _eventBus = eventBus;
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;
            TotalScore += amount;
            _eventBus.Publish(new ScoreChangedEvent { Score = CurrentScore });
        }

        public void ResetLevelScore()
        {
            CurrentScore = 0;
            _eventBus.Publish(new ScoreChangedEvent { Score = CurrentScore });
        }

        public int CalculateLevelScore(bool noCollisions, float energyUsedPercent)
        {
            float score = _config.BaseScorePerLevel;
            if (noCollisions)
                score *= _config.NoCollisionMultiplier;
            if (energyUsedPercent < _config.LowEnergyThreshold)
                score *= _config.LowEnergyBonusMultiplier;
            return (int)score;
        }
    }
}
