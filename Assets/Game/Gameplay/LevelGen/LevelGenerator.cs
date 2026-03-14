using System.Collections.Generic;
using GravityThread.Configs;
using UnityEngine;

namespace GravityThread.Gameplay.LevelGen
{
    public sealed class LevelGenerator
    {
        private readonly LevelGenerationConfig _config;

        public LevelGenerator(LevelGenerationConfig config)
        {
            _config = config;
        }

        public LevelData Generate(int levelIndex, int seed = -1)
        {
            if (seed < 0) seed = Random.Range(0, int.MaxValue);
            var rng = new System.Random(seed);

            int width = _config.GridWidth;
            int height = _config.GridHeight;

            var data = new LevelData
            {
                Width = width,
                Height = height,
                Grid = new CellType[width, height]
            };

            // Fill all with walls
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    data.Grid[x, y] = CellType.Wall;

            // Generate winding path from bottom to top
            var path = GeneratePath(width, height, rng, levelIndex);
            data.PathCells = path;

            // Carve path
            int pathWidth = Mathf.Max(1, _config.PathWidth - Mathf.FloorToInt(levelIndex * _config.NarrowingPerLevel));
            foreach (var cell in path)
            {
                for (int dx = 0; dx < pathWidth; dx++)
                {
                    for (int dy = 0; dy < pathWidth; dy++)
                    {
                        int px = cell.x + dx;
                        int py = cell.y + dy;
                        if (px >= 0 && px < width && py >= 0 && py < height)
                            data.Grid[px, py] = CellType.Path;
                    }
                }
            }

            // Place start and goal
            data.StartCell = path[0];
            data.GoalCell = path[path.Count - 1];
            data.Grid[data.StartCell.x, data.StartCell.y] = CellType.Start;
            data.Grid[data.GoalCell.x, data.GoalCell.y] = CellType.Goal;

            // Place obstacles along path
            PlaceObstacles(data, path, rng, levelIndex);

            return data;
        }

        private List<Vector2Int> GeneratePath(int width, int height, System.Random rng, int levelIndex)
        {
            var path = new List<Vector2Int>();

            int x = width / 2;
            int y = 0;
            path.Add(new Vector2Int(x, y));

            int direction = 0; // 0 = up, 1 = right, -1 = left

            while (y < height - 2)
            {
                int straightLen = rng.Next(_config.MinStraightLength, _config.MaxStraightLength + 1);

                if (direction == 0) // Move up
                {
                    for (int i = 0; i < straightLen && y < height - 2; i++)
                    {
                        y++;
                        path.Add(new Vector2Int(x, y));
                    }
                    direction = rng.NextDouble() > 0.5 ? 1 : -1;
                }
                else // Move horizontally
                {
                    for (int i = 0; i < straightLen; i++)
                    {
                        x += direction;
                        x = Mathf.Clamp(x, 1, width - 3);
                        path.Add(new Vector2Int(x, y));
                    }
                    direction = 0; // Go back to up
                }
            }

            return path;
        }

        private void PlaceObstacles(LevelData data, List<Vector2Int> path, System.Random rng, int levelIndex)
        {
            float spikeChance = _config.SpikeChance + levelIndex * _config.SpikeChancePerLevel;
            bool colorGatePlaced = false;

            for (int i = 2; i < path.Count - 2; i++)
            {
                var cell = path[i];
                if (data.Grid[cell.x, cell.y] != CellType.Path) continue;

                float roll = (float)rng.NextDouble();

                if (!colorGatePlaced && roll < _config.ColorGateChance && i > path.Count / 3)
                {
                    data.Grid[cell.x, cell.y] = CellType.ColorGate;
                    colorGatePlaced = true;

                    // Place color source earlier in path
                    int sourceIdx = Mathf.Max(2, i / 2);
                    var sourceCell = path[sourceIdx];
                    if (data.Grid[sourceCell.x, sourceCell.y] == CellType.Path)
                        data.Grid[sourceCell.x, sourceCell.y] = CellType.ColorSource;
                }
                else if (roll < spikeChance)
                {
                    // Place spike adjacent to path (on wall)
                    TryPlaceSpikeAdjacent(data, cell, rng);
                }
                else if (roll < spikeChance + _config.PulsingWallChance)
                {
                    TryPlacePulsingWallAdjacent(data, cell, rng);
                }
                else if (roll < spikeChance + _config.PulsingWallChance + _config.BottleChance)
                {
                    data.Grid[cell.x, cell.y] = CellType.Bottle;
                }
            }
        }

        private void TryPlaceSpikeAdjacent(LevelData data, Vector2Int cell, System.Random rng)
        {
            var offsets = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            ShuffleArray(offsets, rng);

            foreach (var offset in offsets)
            {
                int nx = cell.x + offset.x;
                int ny = cell.y + offset.y;
                if (nx >= 0 && nx < data.Width && ny >= 0 && ny < data.Height && data.Grid[nx, ny] == CellType.Wall)
                {
                    data.Grid[nx, ny] = CellType.Spike;
                    return;
                }
            }
        }

        private void TryPlacePulsingWallAdjacent(LevelData data, Vector2Int cell, System.Random rng)
        {
            var offsets = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            ShuffleArray(offsets, rng);

            foreach (var offset in offsets)
            {
                int nx = cell.x + offset.x;
                int ny = cell.y + offset.y;
                if (nx >= 0 && nx < data.Width && ny >= 0 && ny < data.Height && data.Grid[nx, ny] == CellType.Wall)
                {
                    data.Grid[nx, ny] = CellType.PulsingWall;
                    return;
                }
            }
        }

        private static void ShuffleArray<T>(T[] array, System.Random rng)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                T tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }
    }
}
