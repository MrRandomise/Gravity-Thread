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
                            CreateWall(worldPos, cellSize);
                            break;
                        case CellType.Spike:
                            var spike = Instantiate(_config.SpikePrefab, worldPos, "Spike", cellSize);
                            if (spike != null) spike.tag = "Spike";
                            break;
                        case CellType.ColorGate:
                            var gate = Instantiate(_config.ColorGatePrefab, worldPos, "ColorGate", cellSize);
                            if (gate != null)
                            {
                                gate.tag = "ColorGate";
                                var gateView = gate.GetComponent<ColorGateView>();
                                if (gateView == null) gateView = gate.AddComponent<ColorGateView>();
                            }
                            break;
                        case CellType.PulsingWall:
                            var pulsingWall = Instantiate(_config.PulsingWallPrefab, worldPos, "PulsingWall", cellSize);
                            if (pulsingWall != null)
                            {
                                pulsingWall.tag = "Wall";
                                var pulseView = pulsingWall.GetComponent<PulsingWallView>();
                                if (pulseView == null) pulseView = pulsingWall.AddComponent<PulsingWallView>();
                            }
                            break;
                        case CellType.Bottle:
                            var bottle = Instantiate(_config.BottlePrefab, worldPos, "Bottle", cellSize);
                            if (bottle != null)
                            {
                                bottle.tag = "Bottle";
                                if (bottle.GetComponent<BottleView>() == null)
                                    bottle.AddComponent<BottleView>();
                            }
                            break;
                        case CellType.Goal:
                            CreateGoal(worldPos, cellSize);
                            break;
                        case CellType.Start:
                            startPos = worldPos;
                            break;
                        case CellType.ColorSource:
                            var colorSrc = Instantiate(_config.ColorSourcePrefab, worldPos, "ColorSource", cellSize);
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

        /// <summary>
        /// Creates a wall cell with a collider that covers the full cell (no gaps).
        /// </summary>
        private void CreateWall(Vector3 position, float cellSize)
        {
            if (_config.WallPrefab != null)
            {
                var obj = Object.Instantiate(_config.WallPrefab, position, Quaternion.identity, _levelRoot.transform);
                obj.name = "Wall";
                // Ensure wall collider covers full cell
                var box = obj.GetComponent<BoxCollider2D>();
                if (box != null)
                    box.size = Vector2.one * cellSize / obj.transform.localScale.x;
                return;
            }

            // Placeholder wall — full cell size, no gaps
            var placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            placeholder.transform.position = position;
            placeholder.transform.localScale = Vector3.one * cellSize;
            placeholder.transform.SetParent(_levelRoot.transform);
            placeholder.name = "Wall";

            Object.Destroy(placeholder.GetComponent<Collider>());
            var collider = placeholder.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            placeholder.tag = "Wall";
        }

        /// <summary>
        /// Creates a clearly visible green goal with a trigger collider.
        /// </summary>
        private void CreateGoal(Vector3 position, float cellSize)
        {
            if (_config.GoalPrefab != null)
            {
                var obj = Object.Instantiate(_config.GoalPrefab, position, Quaternion.identity, _levelRoot.transform);
                obj.name = "Goal";
                obj.tag = "Goal";

                // Ensure goal has a trigger
                var col = obj.GetComponent<Collider2D>();
                if (col != null) col.isTrigger = true;
                return;
            }

            // Placeholder goal — green, with trigger
            var placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            placeholder.transform.position = position;
            placeholder.transform.localScale = Vector3.one * cellSize * 0.9f;
            placeholder.transform.SetParent(_levelRoot.transform);
            placeholder.name = "Goal";
            placeholder.tag = "Goal";

            // Green visual
            var renderer = placeholder.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.green;

            // Replace 3D collider with 2D trigger
            Object.Destroy(placeholder.GetComponent<Collider>());
            var trigger = placeholder.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
        }

        private GameObject Instantiate(GameObject prefab, Vector3 position, string fallbackTag, float cellSize)
        {
            if (prefab == null)
            {
                _debug.LogWarning($"Missing prefab for '{fallbackTag}'. Creating placeholder.");
                var placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placeholder.transform.position = position;
                placeholder.transform.localScale = Vector3.one * cellSize;
                placeholder.transform.SetParent(_levelRoot.transform);
                placeholder.name = fallbackTag;

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
