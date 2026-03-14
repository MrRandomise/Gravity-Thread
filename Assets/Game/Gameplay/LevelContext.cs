using GravityThread.Configs;
using GravityThread.Core;
using GravityThread.Core.Events;
using GravityThread.Services.Score;
using UnityEngine;
using Zenject;

namespace GravityThread.Gameplay
{
    public sealed class LevelContext : MonoBehaviour
    {
        private GameStateManager _stateManager;
        private IScoreService _scoreService;
        private EventBus _eventBus;
        private BallController _ball;
        private ThreadSystem _threadSystem;
        private GameConfig _gameConfig;
        private ThreadConfig _threadConfig;
        private int _currentLevelIndex;

        [Inject]
        public void Construct(
            GameStateManager stateManager,
            IScoreService scoreService,
            EventBus eventBus,
            BallController ball,
            ThreadSystem threadSystem,
            GameConfig gameConfig,
            ThreadConfig threadConfig)
        {
            _stateManager = stateManager;
            _scoreService = scoreService;
            _eventBus = eventBus;
            _ball = ball;
            _threadSystem = threadSystem;
            _gameConfig = gameConfig;
            _threadConfig = threadConfig;
        }

        public void StartLevel(int levelIndex)
        {
            _currentLevelIndex = levelIndex;
            _scoreService.ResetLevelScore();
            _threadSystem.Reset();
            _stateManager.SetState(GameState.Playing);
            _eventBus.Publish(new LevelStartedEvent { LevelIndex = levelIndex });
        }

        public void OnGoalReached()
        {
            if (_stateManager.CurrentState != GameState.Playing) return;

            float energyUsedPercent = _threadSystem.TotalEnergyUsed / _threadConfig.MaxEnergy;
            bool noCollisions = !_ball.HadCollision;

            int score = _scoreService.CalculateLevelScore(noCollisions, energyUsedPercent);
            _scoreService.AddScore(score);

            _stateManager.SetState(GameState.LevelComplete);
            _eventBus.Publish(new LevelCompletedEvent
            {
                LevelIndex = _currentLevelIndex,
                Score = score,
                NoCollisions = noCollisions,
                EnergyUsedPercent = energyUsedPercent
            });
        }

        public void OnBallDestroyed()
        {
            _stateManager.SetState(GameState.LevelFailed);
            _eventBus.Publish(new LevelFailedEvent { LevelIndex = _currentLevelIndex });
        }
    }
}
