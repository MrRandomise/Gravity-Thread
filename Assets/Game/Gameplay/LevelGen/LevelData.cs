using System.Collections.Generic;
using UnityEngine;

namespace GravityThread.Gameplay.LevelGen
{
    public enum CellType
    {
        Empty,
        Wall,
        Path,
        Spike,
        ColorGate,
        PulsingWall,
        Bottle,
        Goal,
        Start,
        ColorSource
    }

    public sealed class LevelData
    {
        public int Width;
        public int Height;
        public CellType[,] Grid;
        public Vector2Int StartCell;
        public Vector2Int GoalCell;
        public List<Vector2Int> PathCells = new List<Vector2Int>();
        public Color GateColor = Color.green;
    }
}
