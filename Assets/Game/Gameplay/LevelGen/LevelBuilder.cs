using GravityThread.Configs;
using GravityThread.Gameplay.Views;
using GravityThread.Services.Debug;
using UnityEngine;

namespace GravityThread.Gameplay.LevelGen
{
    public sealed class LevelBuilder
    {
        private readonly LevelGenerationConfig _config;
        private readonly GameConfig _gameConfig;
        private readonly IDebugService _debug;

        private GameObject _levelRoot;

        public LevelBuilder(LevelGenerationConfig config, GameConfig gameConfig, IDebugService debug)
        {
            _config = config;
            _gameConfig = gameConfig;
            _debug = debug;
        }

        public Vector2 Build(LevelData data)
        {
            DestroyCurrentLevel();
            _levelRoot = new GameObject("Level_Root");

            float cellSize = _config.CellSize;
            Vector2 startPos = Vector2.zero;

            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    Vector3 worldPos = new Vector3(x * cellSize, y * cellSize, 0);
                    var cellType = data.Grid[x, y];

                    switch (cellType)
                    {
                        case CellType.Wall:
                            Instantiate(_config.WallPrefab, worldPos, "Wall");
                            break;
                        case CellType.Spike:
                            var spike = Instantiate(_config.SpikePrefab, worldPos, "Spike");
                            if (spike != null) spike.tag = "Spike";
                            break;
                        case CellType.ColorGate:
                            var gate = Instantiate(_config.ColorGatePrefab, worldPos, "ColorGate");
                            if (gate != null)
                            {
                                gate.tag = "ColorGate";
                                var gateView = gate.GetComponent<ColorGateView>();
                                if (gateView == null) gateView = gate.AddComponent<ColorGateView>();
                            }
                            break;
                        case CellType.PulsingWall:
                            var pulsingWall = Instantiate(_config.PulsingWallPrefab, worldPos, "PulsingWall");
                            if (pulsingWall != null)
                            {
                                pulsingWall.tag = "Wall";
                                var pulseView = pulsingWall.GetComponent<PulsingWallView>();
                                if (pulseView == null) pulseView = pulsingWall.AddComponent<PulsingWallView>();
                            }
                            break;
                        case CellType.Bottle:
                            var bottle = Instantiate(_config.BottlePrefab, worldPos, "Bottle");
                            if (bottle != null)
                            {
                                bottle.tag = "Bottle";
                                if (bottle.GetComponent<BottleView>() == null)
                                    bottle.AddComponent<BottleView>();
                            }
                            break;
                        case CellType.Goal:
                            var goal = Instantiate(_config.GoalPrefab, worldPos, "Goal");
                            if (goal != null) goal.tag = "Goal";
                            break;
                        case CellType.Start:
                            startPos = worldPos;
                            break;
                        case CellType.ColorSource:
                            var colorSrc = Instantiate(_config.ColorSourcePrefab, worldPos, "ColorSource");
                            if (colorSrc != null)
                            {
                                colorSrc.tag = "ColorSource";
                                var srcView = colorSrc.GetComponent<ColorSourceView>();
                                if (srcView == null) srcView = colorSrc.AddComponent<ColorSourceView>();
                            }
                            break;
                    }
                }
            }

            _debug.Log($"Level built: {data.Width}x{data.Height}, Start: {data.StartCell}, Goal: {data.GoalCell}");
            return startPos;
        }

        public void DestroyCurrentLevel()
        {
            if (_levelRoot != null)
                Object.Destroy(_levelRoot);
        }

        private GameObject Instantiate(GameObject prefab, Vector3 position, string fallbackTag)
        {
            if (prefab == null)
            {
                _debug.LogWarning($"Missing prefab for '{fallbackTag}'. Creating placeholder.");
                var placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.transform.position = position;
                placeholder.transform.localScale = Vector3.one * _config.CellSize * 0.9f;
                placeholder.transform.SetParent(_levelRoot.transform);
                placeholder.name = fallbackTag;

                // Remove 3D collider, add 2D
                Object.Destroy(placeholder.GetComponent<Collider>());
                placeholder.AddComponent<BoxCollider2D>();
                return placeholder;
            }

            var obj = Object.Instantiate(prefab, position, Quaternion.identity, _levelRoot.transform);
            obj.name = fallbackTag;
            return obj;
        }
    }
}
