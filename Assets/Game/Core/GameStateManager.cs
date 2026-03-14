using GravityThread.Core.Events;

namespace GravityThread.Core
{
    public sealed class GameStateManager : IGameStateProvider
    {
        private readonly EventBus _eventBus;
        private readonly GameLifecycleRunner _runner;

        public GameState CurrentState { get; private set; } = GameState.None;

        public GameStateManager(EventBus eventBus, GameLifecycleRunner runner)
        {
            _eventBus = eventBus;
            _runner = runner;
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;

            switch (newState)
            {
                case GameState.Paused:
                    _runner.IsPaused = true;
                    _eventBus.Publish(new GamePausedEvent { IsPaused = true });
                    break;
                case GameState.Playing:
                    _runner.IsPaused = false;
                    _eventBus.Publish(new GamePausedEvent { IsPaused = false });
                    break;
            }
        }
    }
}
