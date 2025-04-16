using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using PuzzlePiece;
using PuzzlePiece.Features;

namespace PuzzleData.Save
{
    [Serializable]
    public class PieceConfigurationSave
    {
        private List<int> _features;

        public List<int> Features => _features;
        
        [JsonConstructor]
        public PieceConfigurationSave(List<int> features)
        {
            _features = features;
        }

        public PieceConfigurationSave(PieceConfiguration pieceConfiguration)
        {
            _features = new List<int>
            {
                (int)pieceConfiguration.Left,
                (int)pieceConfiguration.Top,
                (int)pieceConfiguration.Right,
                (int)pieceConfiguration.Bottom
            };
        }

        public PieceConfiguration ConvertToPieceConfiguration()
        {
            return new PieceConfiguration((FeatureType)_features[0], (FeatureType)_features[1], (FeatureType)_features[2], (FeatureType)_features[3]);
        }

    }
}