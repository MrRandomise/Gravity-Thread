using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GravityThread.Services.Localization
{
    [CreateAssetMenu(fileName = "LocalizationTable", menuName = "GravityThread/Localization Table")]
    public sealed class LocalizationTable : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "Key", ValueLabel = "Text")]
        public Dictionary<string, string> Entries = new Dictionary<string, string>();
    }
}
