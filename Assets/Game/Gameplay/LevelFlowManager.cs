using GravityThread.Core;
using GravityThread.Core.Events;
using GravityThread.Gameplay.LevelGen;
using GravityThread.Services.Debug;
using UnityEngine;

namespace GravityThread.Gameplay
{
    public sealed class LevelFlowManager
    {
        private readonly LevelGenerator _generator;
        private readonly LevelBuilder _builder;
        private readonly EventBus _eventBus;
        private readonly IDebugService _debug;

        private int _currentLevelIndex;

        public int CurrentLevelIndex => _currentLevelIndex;

        public LevelFlowManager(
            LevelGenerator generator,
            LevelBuilder builder,
            EventBus eventBus,
            IDebugService debug)
        {
            _generator = generator;
            _builder = builder;
            _eventBus = eventBus;
            _debug = debug;
        }

        public Vector2 LoadLevel(int levelIndex)
        {
            _currentLevelIndex = levelIndex;
            var data = _generator.Generate(levelIndex);
            var startPos = _builder.Build(data);
            _debug.Log($"Level {levelIndex} loaded. Ball start position: {startPos}");
            return startPos;
        }

        public Vector2 LoadNextLevel()
        {
            return LoadLevel(_currentLevelIndex + 1);
        }

        public void UnloadLevel()
        {
            _builder.DestroyCurrentLevel();
        }
    }
}
