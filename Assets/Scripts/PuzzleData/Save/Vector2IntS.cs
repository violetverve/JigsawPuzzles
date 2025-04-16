using UnityEngine;
using System;
using Newtonsoft.Json;

namespace PuzzleData.Save
{
    [Serializable]
    public class Vector2IntS
    {
        private int _x, _y;

        public int X => _x;
        public int Y => _y;

        public Vector2IntS(Vector2Int vector2Int)
        {
            _x = vector2Int.x;
            _y = vector2Int.y;
        }

        [JsonConstructor]
        public Vector2IntS(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(_x, _y);
        }
    }
}
