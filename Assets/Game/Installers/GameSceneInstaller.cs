using GravityThread.Configs;
using GravityThread.Core;
using GravityThread.Gameplay;
using GravityThread.Gameplay.LevelGen;
using GravityThread.Services.Achievements;
using GravityThread.Services.Audio;
using GravityThread.Services.Debug;
using GravityThread.Services.Input;
using UnityEngine;
using Zenject;

namespace GravityThread.Installers
{
    public sealed class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private AudioConfig _audioConfig;

        public override void InstallBindings()
        {
            // Lifecycle runner (single MonoBehaviour entry point)
            var runnerGo = new GameObject("[GameLifecycleRunner]");
            DontDestroyOnLoad(runnerGo);
            var runner = runnerGo.AddComponent<GameLifecycleRunner>();
            Container.BindInstance(runner).AsSingle();

            // Game state
            Container.Bind<GameStateManager>().AsSingle();
            Container.Bind<IGameStateProvider>().To<GameStateManager>().FromResolve();

            // Input
            if (_mainCamera == null)
                _mainCamera = Camera.main;
            Container.Bind<IInputService>().To<InputService>().AsSingle().WithArguments(_mainCamera);

            // Gameplay
            Container.Bind<ThreadSystem>().AsSingle();
            Container.Bind<BallController>().AsSingle();
            Container.Bind<CameraFollowSystem>().AsSingle();
            Container.Bind<TrailMaterialController>().AsSingle();

            // Level
            Container.Bind<LevelGenerator>().AsSingle();
            Container.Bind<LevelBuilder>().AsSingle();
            Container.Bind<LevelFlowManager>().AsSingle();

            Container.Bind<LevelContext>().FromComponentInHierarchy().AsSingle();

            // Achievements
            Container.Bind<IAchievementService>().To<AchievementService>().AsSingle();

            // Audio
            Container.BindInstance(_audioConfig).AsSingle();
            Container.Bind<AudioService>().AsSingle().NonLazy();
        }

        public override void Start()
        {
            var runner = Container.Resolve<GameLifecycleRunner>();
            var inputService = Container.Resolve<IInputService>();
            var threadSystem = Container.Resolve<ThreadSystem>();
            var ballController = Container.Resolve<BallController>();
            var cameraFollow = Container.Resolve<CameraFollowSystem>();
            var trailMaterial = Container.Resolve<TrailMaterialController>();

            runner.Register(inputService);
            runner.Register(threadSystem);
            runner.Register(ballController);
            runner.Register(cameraFollow);
            runner.Register(trailMaterial);

            // Start background music
            var audioService = Container.Resolve<AudioService>();
            audioService.StartMusic();

            var debug = Container.Resolve<IDebugService>();
            debug.Log("GameSceneInstaller: All systems registered with lifecycle runner.");
        }
    }
}