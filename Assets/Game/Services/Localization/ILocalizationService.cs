namespace GravityThread.Services.Localization
{
    public interface ILocalizationService
    {
        string CurrentLanguage { get; }
        void SetLanguage(string languageCode);
        string Get(string key);
        string Get(string key, params object[] args);
    }
}
