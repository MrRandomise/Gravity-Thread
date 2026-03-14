namespace GravityThread.Core
{
    public enum GameState
    {
        None,
        MainMenu,
        Playing,
        Paused,
        LevelComplete,
        LevelFailed
    }

    public interface IGameStateProvider
    {
        GameState CurrentState { get; }
        void SetState(GameState newState);
    }
}
