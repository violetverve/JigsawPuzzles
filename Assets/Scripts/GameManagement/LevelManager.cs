using UnityEngine;
using System;
using Player;
using PuzzleData.Save;

namespace GameManagement
{
    public class LevelManager : MonoBehaviour
    {
        public static Action<Level> LevelStarted;
        public static Action<Level, PuzzleSave> LevelLoaded;
        [SerializeField] private LevelDebugShell _debugLevel;

        private Level _currentLevel;
        public Level CurrentLevel => _currentLevel;

        private void Awake()
        {
            _currentLevel = PlayerData.Instance.CurrentLevel;
            Debug.Log($"Current level: {_currentLevel.PuzzleData.Id}");

            if (TryGetSavedPuzzle(_currentLevel.PuzzleData.Id, out PuzzleSave savedPuzzle))
            {
                Debug.Log("Level previously saved. Loading ...");
                LoadLevel(_currentLevel, savedPuzzle);
            }
            else
            {
                Debug.Log("Level not previously saved. Starting ...");
                StartLevel(_currentLevel);
            }
        }

        private bool TryGetSavedPuzzle(int puzzleId, out PuzzleSave savedPuzzle)
        {
            savedPuzzle = PlayerData.Instance.TryGetSavedPuzzle(puzzleId);
            return savedPuzzle != null;
        }


        private void StartLevel(Level level)
        {
            LevelStarted?.Invoke(level);
        }

        private void LoadLevel(Level level, PuzzleSave puzzleSave)
        {
            LevelLoaded?.Invoke(level, puzzleSave);
        }

    }
}
