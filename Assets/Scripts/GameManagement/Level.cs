using Grid;
using JigsawPuzzles.PuzzleData;

namespace GameManagement
{
    public class Level
    {
        private GridSO _gridSO;
        private PuzzleDocument _puzzleData;
        private bool _rotationEnabled;

        public GridSO GridSO => _gridSO;
        public PuzzleDocument PuzzleData => _puzzleData;
        public bool RotationEnabled => _rotationEnabled;

        public Level(GridSO gridSO, PuzzleDocument puzzleData, bool rotationEnabled)
        {
            _gridSO = gridSO;
            _puzzleData = puzzleData;
            _rotationEnabled = rotationEnabled;
        }
    }
}