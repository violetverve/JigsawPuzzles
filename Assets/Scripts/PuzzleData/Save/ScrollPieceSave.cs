using System;
using PuzzlePiece;
using Newtonsoft.Json;

namespace PuzzleData.Save
{
    [Serializable]
    public class ScrollPieceSave
    {
        private Vector2IntS _gridPosition;
        private Vector3S _rotation;
        
        public Vector2IntS GridPosition => _gridPosition;
        public Vector3S Rotation => _rotation;

        public ScrollPieceSave(Piece piece)
        {
            _gridPosition = new Vector2IntS(piece.GridPosition);
            _rotation = new Vector3S(piece.Transform.eulerAngles);
        }

        [JsonConstructor]
        public ScrollPieceSave(Vector2IntS gridPosition, Vector3S rotation)
        {
            _gridPosition = gridPosition;
            _rotation = rotation;
        }
    }
}
