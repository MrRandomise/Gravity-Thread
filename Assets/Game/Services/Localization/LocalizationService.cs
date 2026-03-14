using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

namespace GravityThread.Services.Localization
{
    public sealed class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, LocalizationTable> _tables;
        private LocalizationTable _currentTable;

        public string CurrentLanguage { get; private set; }

        public LocalizationService(Dictionary<string, LocalizationTable> tables, string defaultLanguage = "en")
        {
            _tables = tables;
            SetLanguage(defaultLanguage);
        }

        public void SetLanguage(string languageCode)
        {
            if (_tables.TryGetValue(languageCode, out var table))
            {
                CurrentLanguage = languageCode;
                _currentTable = table;
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[Localization] Language '{languageCode}' not found. Falling back to first available.");
                foreach (var kvp in _tables)
                {
                    CurrentLanguage = kvp.Key;
                    _currentTable = kvp.Value;
                    break;
                }
            }
        }

        public string Get(string key)
        {
            if (_currentTable != null && _currentTable.Entries.TryGetValue(key, out var value))
                return value;
            return $"[{key}]";
        }

        public string Get(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }
    }
}
