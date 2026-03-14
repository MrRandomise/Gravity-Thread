using GravityThread.Core;
using GravityThread.Core.Events;
using GravityThread.Gameplay;
using GravityThread.Services.Achievements;
using GravityThread.Services.Debug;
using UnityEngine;
using Zenject;

namespace GravityThread.Bootstrap
{
    /// <summary>
    /// Main game bootstrapper. Orchestrates the startup sequence.
    /// Placed on a scene object with Zenject injection.
    /// </summary>
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private GameStateManager _stateManager;
        private LevelFlowManager _levelFlow;
        private LevelContext _levelContext;
        private EventBus _eventBus;
        private BallController _ball;
        private IDebugService _debug;
        private CameraFollowSystem _cameraFollow;
        private GameLifecycleRunner _runner;

        [Inject]
        public void Construct(
            GameStateManager stateManager,
            LevelFlowManager levelFlow,
            LevelContext levelContext,
            EventBus eventBus,
            BallController ball,
            IDebugService debug,
            CameraFollowSystem cameraFollow,
            GameLifecycleRunner runner)
        {
            _stateManager = stateManager;
            _levelFlow = levelFlow;
            _levelContext = levelContext;
            _eventBus = eventBus;
            _ball = ball;
            _debug = debug;
            _cameraFollow = cameraFollow;
            _runner = runner;
        }

        private void Start()
        {
            _debug.Log("GameBootstrapper: Initializing...");

            _cameraFollow.Initialize(Camera.main.transform);
            _runner.Register(_cameraFollow);

            _eventBus.Subscribe<BallDestroyedEvent>(OnBallDestroyed);
            _eventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);

            StartGame();
        }

        private void StartGame()
        {
            var startPos = _levelFlow.LoadLevel(0);
            _levelContext.StartLevel(0);
            _debug.Log($"Game started. Ball at {startPos}");
        }

        private void OnBallDestroyed(BallDestroyedEvent e)
        {
            _levelContext.OnBallDestroyed();
        }

        private void OnLevelCompleted(LevelCompletedEvent e)
        {
            _debug.Log($"Level {e.LevelIndex} completed. Score: {e.Score}");
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<BallDestroyedEvent>(OnBallDestroyed);
            _eventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
        }
    }
}
