using GravityThread.Core.Events;
using GravityThread.Gameplay;
using GravityThread.Services.Debug;
using UnityEngine;
using Zenject;

namespace GravityThread.Debug
{
    public sealed class DebugOverlay : MonoBehaviour
    {
        private IDebugService _debugService;
        private BallController _ball;
        private ThreadSystem _thread;
        private EventBus _eventBus;

        private bool _showOverlay;
        private string _lastEvent = "";

        [Inject]
        public void Construct(IDebugService debugService, BallController ball, ThreadSystem thread, EventBus eventBus)
        {
            _debugService = debugService;
            _ball = ball;
            _thread = thread;
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<BallWallCollisionEvent>(e => _lastEvent = $"Wall hit: {e.ImpactForce:F1}");
            _eventBus.Subscribe<SpikeHitEvent>(e => _lastEvent = "Spike hit!");
            _eventBus.Subscribe<BottleCollectedEvent>(e => _lastEvent = "Bottle collected!");
            _eventBus.Subscribe<LevelCompletedEvent>(e => _lastEvent = $"Level {e.LevelIndex} complete! Score: {e.Score}");
            _eventBus.Subscribe<LevelFailedEvent>(e => _lastEvent = $"Level {e.LevelIndex} failed!");
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
                _showOverlay = !_showOverlay;
        }

        private void OnGUI()
        {
            if (!_debugService.IsDebugEnabled || !_showOverlay) return;

            float x = 10, y = 10, w = 280, h = 20;

            GUI.color = Color.white;
            GUI.Label(new Rect(x, y, w, h), $"[DEBUG] F1 to toggle");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"Ball Pos: {_ball.Position}");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"Ball Speed: {_ball.Speed:F2}");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"Ball Alive: {_ball.IsAlive}");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"Thread Energy: {_thread.NormalizedEnergy:P0}");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"Thread Active: {_thread.IsActive}");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"Last Event: {_lastEvent}");
            y += h;
            GUI.Label(new Rect(x, y, w, h), $"FPS: {1f / Time.unscaledDeltaTime:F0}");
        }
    }
}
