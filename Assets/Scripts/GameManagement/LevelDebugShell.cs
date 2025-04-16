using UnityEngine;
using PuzzleData;
using Grid;

namespace GameManagement
{
    public class LevelDebugShell : MonoBehaviour
    {
        [SerializeField] private GridSO _gridSO;
        [SerializeField] private PuzzleSO _puzzleSO;
        [SerializeField] private bool _rotationEnabled;

        public GridSO GridSO => _gridSO;
        public PuzzleSO PuzzleSO => _puzzleSO;
        public bool RotationEnabled => _rotationEnabled;
    }
}