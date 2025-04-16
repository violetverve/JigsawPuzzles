using PuzzlePiece.Features;
using System;

namespace PuzzlePiece {
    [Serializable]
    public struct PieceConfiguration
    {
        private FeatureType _left;
        private FeatureType _top;
        private FeatureType _right;
        private FeatureType _bottom;

        public FeatureType Left => _left;
        public FeatureType Top => _top;
        public FeatureType Right => _right;
        public FeatureType Bottom => _bottom;

        public PieceConfiguration(FeatureType left, FeatureType top, FeatureType right, FeatureType bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
    }
}