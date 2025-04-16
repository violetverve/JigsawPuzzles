using System;
using PuzzlePiece;
using Newtonsoft.Json;


namespace PuzzleData.Save
{
    [Serializable]
    public class PieceSave
    {
        private Vector2IntS _gridPosition;
        private Vector3S _position;
        private Vector3S _rotation;
        
        public Vector2IntS GridPosition => _gridPosition;
        public Vector3S Position => _position;
        public Vector3S Rotation => _rotation;

        public PieceSave(Piece piece)
        {
            _gridPosition = new Vector2IntS(piece.GridPosition);
            _position = new Vector3S(piece.Transform.position);
            _rotation = new Vector3S(piece.Transform.eulerAngles);
        }

        [JsonConstructor]
        public PieceSave(Vector2IntS gridPosition, Vector3S position, Vector3S rotation)
        {
            _gridPosition = gridPosition;
            _position = position;
            _rotation = rotation;
        }
    }
}
