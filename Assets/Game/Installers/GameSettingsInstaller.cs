using System.Collections.Generic;
using GravityThread.Configs;
using GravityThread.Core.Events;
using GravityThread.Services.Debug;
using GravityThread.Services.Localization;
using GravityThread.Services.Score;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace GravityThread.Installers
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "GravityThread/Game Settings Installer")]
    public sealed class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        [Title("Configs")]
        [Required] public BallConfig BallConfig;
        [Required] public ThreadConfig ThreadConfig;
        [Required] public LevelGenerationConfig LevelGenConfig;
        [Required] public GameConfig GameConfig;

        [Title("Localization")]
        [DictionaryDrawerSettings(KeyLabel = "Lang Code", ValueLabel = "Table")]
        [SerializeReference]public Dictionary<string, LocalizationTable> LocalizationTables = new Dictionary<string, LocalizationTable>();
        public string DefaultLanguage = "en";

        [Title("Debug")]
        public bool EnableDebug = true;

        public override void InstallBindings()
        {
            // Configs
            Container.BindInstance(BallConfig).AsSingle();
            Container.BindInstance(ThreadConfig).AsSingle();
            Container.BindInstance(LevelGenConfig).AsSingle();
            Container.BindInstance(GameConfig).AsSingle();

            // Core
            Container.Bind<EventBus>().AsSingle();

            // Services
            Container.Bind<IDebugService>().To<DebugService>().AsSingle()
                .WithArguments(EnableDebug);

            Container.Bind<ILocalizationService>().To<LocalizationService>().AsSingle()
                .WithArguments(LocalizationTables, DefaultLanguage);

            Container.Bind<IScoreService>().To<ScoreService>().AsSingle();
        }
    }
}
