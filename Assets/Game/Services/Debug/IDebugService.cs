namespace GravityThread.Services.Debug
{
    public interface IDebugService
    {
        bool IsDebugEnabled { get; set; }
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
