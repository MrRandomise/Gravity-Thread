namespace GravityThread.Services.Debug
{
    public sealed class DebugService : IDebugService
    {
        public bool IsDebugEnabled { get; set; }

        public DebugService(bool enableByDefault = false)
        {
            IsDebugEnabled = enableByDefault;
        }

        public void Log(string message)
        {
            if (IsDebugEnabled)
                UnityEngine.Debug.Log($"[GravityThread] {message}");
        }

        public void LogWarning(string message)
        {
            if (IsDebugEnabled)
                UnityEngine.Debug.LogWarning($"[GravityThread] {message}");
        }

        public void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[GravityThread] {message}");
        }
    }
}
