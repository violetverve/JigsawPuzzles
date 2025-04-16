using UnityEngine;
using Grid;
using System;

namespace GameManagement
{
    public class ProgressManager : MonoBehaviour
    {
        private readonly float[] _milestones = { 0.25f, 0.5f, 0.75f };

        private int _numberOfPieces;
        private float _lastMilestone = 0f;
        private int _numberOfEdges;
        private bool _edgesCollected = false;

        public static Action EdgesCollected;
        public static Action Win;
        public static Action<float> ProgressUpdated;
        public static Action<float> ProgressLoaded;
        public static Action<bool> EdgesCollectedLoaded;
        
        public bool AreEdgesCollected => _edgesCollected;

        private void OnEnable()
        {
            GridInteractionController.OnProgressUpdate += HandleProgressUpdate;
            LevelManager.LevelLoaded += HandleLevelLoaded;
            LevelManager.LevelStarted += HandleLevelStarted;
        }

        private void OnDisable()
        {
            GridInteractionController.OnProgressUpdate -= HandleProgressUpdate;
            LevelManager.LevelLoaded -= HandleLevelLoaded;
            LevelManager.LevelStarted -= HandleLevelStarted;
        }

        private void HandleLevelStarted(Level level)
        {
            SetNumberOfPieces(level.GridSO);
        }

        private void HandleLevelLoaded(Level level, PuzzleData.Save.PuzzleSave save)
        {
            SetNumberOfPieces(level.GridSO);

            _edgesCollected = save.EdgesCollected;
            EdgesCollectedLoaded?.Invoke(_edgesCollected);

            var progress = CalculateProgressPercent(save.CollectedPieceSaves.Count, _numberOfPieces);
            ProgressLoaded?.Invoke(progress);
        }

        private void HandleProgressUpdate(int numberOfPiecesCollected, int numberOfEdgesCollected)
        {
            CheckForMilestones(numberOfPiecesCollected);
            CheckIfEdgesCollected(numberOfEdgesCollected);

            var progress = CalculateProgressPercent(numberOfPiecesCollected, _numberOfPieces);
            ProgressUpdated?.Invoke(progress);
        }

        public void SetNumberOfPieces(GridSO grid)
        {
            _numberOfPieces = grid.Area;
            _numberOfEdges = grid.Edges;
        }

        private void CheckForMilestones(int numberOfPiecesCollected)
        {
            if (numberOfPiecesCollected == _numberOfPieces)
            {
                Win?.Invoke();
                Debug.Log("All pieces collected!");
            }
            else
            {
                float percentage = (float)numberOfPiecesCollected / _numberOfPieces;

                foreach (float milestone in _milestones)
                {
                    if (percentage >= milestone && _lastMilestone < milestone)
                    {
                        Debug.Log($"Puzzle progress: {milestone * 100}%");
                        _lastMilestone = milestone;
                    }
                }
            }
        }

        private void CheckIfEdgesCollected(int numberOfEdgesCollected)
        {
            if (_edgesCollected) return;
            
            if (numberOfEdgesCollected == _numberOfEdges)
            {
                EdgesCollected?.Invoke();
                _edgesCollected = true;
            }
        }

        private float CalculateProgressPercent(int collectedPieces, int piecesNumber)
        {
            return (float)collectedPieces / piecesNumber;
        }


    }
}