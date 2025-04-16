using System.Collections.Generic;
using PuzzlePiece;
using System;
using Newtonsoft.Json;

namespace PuzzleData.Save
{
    [Serializable]
    public class PuzzleSave
    {
        private int _id;
        private int _gridSide;
        private bool _rotationEnabled;
        private List<List<PieceConfigurationSave>> _pieceConfigurationList;
        private List<Vector2IntS> _collectedPieceSaves;
        private List<SnappableSave> _snappableSaves;
        private List <ScrollPieceSave> _scrollPieceSaves;
        private bool _edgesCollected;

        public int Id => _id;
        public int GridSide => _gridSide;
        public bool RotationEnabled => _rotationEnabled;
        public List<List<PieceConfigurationSave>> PieceConfigurationList => _pieceConfigurationList;
        public List<Vector2IntS> CollectedPieceSaves => _collectedPieceSaves;
        public List<SnappableSave> SnappableSaves => _snappableSaves;
        public List<ScrollPieceSave> ScrollPieceSaves => _scrollPieceSaves;
        public bool EdgesCollected => _edgesCollected;


        [JsonConstructor]
        public PuzzleSave(int id, int gridSide, bool rotationEnabled, List<List<PieceConfigurationSave>> pieceConfigurationList, List<SnappableSave> snappableSaves = null, List<Vector2IntS> collectedPieceSaves = null, List<ScrollPieceSave> scrollPieceSaves = null, bool edgesCollected = false)
        {
            _id = id;
            _gridSide = gridSide;
            _rotationEnabled = rotationEnabled;
            _pieceConfigurationList = pieceConfigurationList;
            _snappableSaves = snappableSaves;
            _collectedPieceSaves = collectedPieceSaves;
            _scrollPieceSaves = scrollPieceSaves;
            _edgesCollected = edgesCollected;
        }

        public PuzzleSave(int id, int gridSide, bool rotationEnabled, PieceConfiguration[,] pieceConfiguration, List<SnappableSave> snappableSaves, List<Vector2IntS> collectedPieceSaves, List<ScrollPieceSave> scrollPieceSaves, bool edgesCollected)
        {
            _id = id;
            _gridSide = gridSide;
            _rotationEnabled = rotationEnabled;
            _pieceConfigurationList = Convert2DArrayToListOfLists(pieceConfiguration);
            _snappableSaves = snappableSaves;
            _collectedPieceSaves = collectedPieceSaves;
            _scrollPieceSaves = scrollPieceSaves;
            _edgesCollected = edgesCollected;
        }

        private List<List<PieceConfigurationSave>> Convert2DArrayToListOfLists(PieceConfiguration[,] pieceConfiguration)
        {
            var pieceConfigurationList = new List<List<PieceConfigurationSave>>();

            for (int i = 0; i < pieceConfiguration.GetLength(0); i++)
            {
                var row = new List<PieceConfigurationSave>();
                for (int j = 0; j < pieceConfiguration.GetLength(1); j++)
                {
                    row.Add(new PieceConfigurationSave(pieceConfiguration[i, j]));
                }
                pieceConfigurationList.Add(row);
            }

            return pieceConfigurationList;
        }

        public PieceConfiguration[,] Get2DArray()
        {
            var pieceConfiguration = new PieceConfiguration[_pieceConfigurationList.Count, _pieceConfigurationList[0].Count];
            for (int i = 0; i < _pieceConfigurationList.Count; i++)
            {
                for (int j = 0; j < _pieceConfigurationList[i].Count; j++)
                {
                    pieceConfiguration[i, j] = _pieceConfigurationList[i][j].ConvertToPieceConfiguration();
                }
            }

            return pieceConfiguration;
        }
    }

}
