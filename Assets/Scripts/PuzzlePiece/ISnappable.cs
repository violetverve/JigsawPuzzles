using System.Collections.Generic;
using UnityEngine;

namespace PuzzlePiece {
    public interface ISnappable
    {
        Transform Transform { get; }
        List<Piece> Pieces { get; }
        Draggable Draggable { get; }
        bool TrySnapToGrid();
        Piece GetNeighbourPiece();
        bool IsSnappedToGrid();
        ISnappable CombineWith(Piece otherPiece);
        void UpdateZPosition(int zPosition);
        void ClampToGrid(GetClampedPositionDelegate getClampedPosition, bool mouseOnScrollView);
        void AddToCollectedPieces(List<Piece> collectedPieces);
        void Rotate(Vector3 mouseWorldPos);
        bool HaveSameRotation(Piece piece);
        void AnimateToCorrectPosition(float duration, int zPosition);
    } 
}

