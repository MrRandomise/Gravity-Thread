namespace GravityThread.Core.Interfaces
{
    public interface IGameTickable
    {
        void GameTick(float deltaTime);
    }

    public interface IGameFixedTickable
    {
        void GameFixedTick(float fixedDeltaTime);
    }

    public interface IGameLateTickable
    {
        void GameLateTick(float deltaTime);
    }
}
