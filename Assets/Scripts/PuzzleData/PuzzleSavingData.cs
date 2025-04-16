using Grid;
using PuzzlePiece;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleData
{
    public class PuzzleSavingData
    {
        private int _id;
        private GridSO _gridSize;
        private List<Vector3> _uncompletedPieces;       

        private PieceConfiguration[,] _pieceConfiguration;

        public PuzzleSavingData(int id, GridSO gridSize)
        {
            _id = id;
            _gridSize = gridSize;
        }

        public int ID => _id;
        public GridSO Grid => _gridSize;
        public List<Vector3> UncompletedPieces => _uncompletedPieces;
        public PieceConfiguration[,] PieceConfigurations => _pieceConfiguration;
    }

}
