using UnityEngine;
using Grid;
using PuzzleData.Save;
using UI.GameScene;
using JigsawPuzzles.UI.MenuScene.Categories;

namespace GameManagement
{
    public class SecretPuzzleManager : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private SpritePaletteSO _spritePaletteSO;
        private Texture _imageTexture;
        private bool _isSecretPuzzle;
        private int _puzzleId;

        private void OnEnable()
        {
            LevelManager.LevelStarted += HandleLevelStarted;
            LevelManager.LevelLoaded += HandleLevelLoaded;
            GridLoader.GridLoaded += HandleGridLoaded;
            GridGenerator.GridGenerated += HandleGridGenerated;
        }

        private void OnDisable()
        {
            GridInteractionController.OnProgressUpdate -= HandleProgressUpdate;
            LevelManager.LevelStarted -= HandleLevelStarted;
            LevelManager.LevelLoaded -= HandleLevelLoaded;
            GridLoader.GridLoaded -= HandleGridLoaded;
            GridGenerator.GridGenerated -= HandleGridGenerated;
        }

        public void HandleGridLoaded()
        {
            if (_isSecretPuzzle)
            {
                UpdatePieces();
            }
        }

        private void HandleGridGenerated()
        {
            if (_isSecretPuzzle)
            {
                SetSecretTexture();
            }
        }

        private void HandleLevelLoaded(Level level, PuzzleSave save)
        {
            SetupUpdates(level);
        }

        private void HandleLevelStarted(Level level)
        {
            SetupUpdates(level);
        }

        private void HandleProgressUpdate(int numberOfPiecesCollected, int numberOfEdgesCollected)
        {
            UpdatePieces();
        }

        private void SetupUpdates(Level level)
        {
            _puzzleId = level.PuzzleData.Id;
            _isSecretPuzzle = level.PuzzleData.IsSecret;
            
            if (!_isSecretPuzzle)
            {
                return;
            }

            _imageTexture = level.PuzzleData.FullSizeSprite.texture;
            GridInteractionController.OnProgressUpdate += HandleProgressUpdate;
        }

        private void SetSecretTexture()
        {
            var notCollectedPieces = _gridManager.GetNotCollectedPieces();

            var texture = _spritePaletteSO.GetSprite(GetSecretLevelID(_puzzleId)).texture;

            foreach (var piece in notCollectedPieces)
            {
                piece.MeshRenderer.material.mainTexture = texture;
            }         
        }

        private void UpdatePieces()
        {
            foreach (var piece in _gridManager.GetCollectedPieces())
            {
                piece.MeshRenderer.material.mainTexture = _imageTexture;
            }
        }
        private int GetSecretLevelID(int puzzleID)
        {
            return (puzzleID - 1000) / ((int)PuzzleCategory.Secret + 1);
        }

    }
}
