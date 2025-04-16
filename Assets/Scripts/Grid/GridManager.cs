using System.Collections.Generic;
using UnityEngine;
using UI.GameScene;
using GameManagement;
using PuzzlePiece;
using PuzzleData.Save;
using System;
using System.Linq;

namespace Grid
{   
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridGenerator _gridGenerator;
        [SerializeField] private GridInteractionController _gridInteractionController;
        [SerializeField] private GridSO _gridSO;
        [SerializeField] private ScrollViewController _scrollViewController;
        [SerializeField] private GridField _gridField;
        [SerializeField] private GridLoader _gridLoader;

        public static Action GridGenerated;

        public GridSO GridSO => _gridSO;
        public GridField GridField => _gridField;
        public GridInteractionController GridInteractionController => _gridInteractionController;
        public GridGenerator GridGenerator => _gridGenerator;
        public ScrollViewController ScrollViewController => _scrollViewController;
        public List<Piece> CollectedPieces => _gridInteractionController.CollectedPieces;
        public PieceConfiguration[,] PieceConfigurations => _gridGenerator.PieceConfigurations;
        
        private void OnEnable()
        {
            LevelManager.LevelStarted += HandleLevelStarted;
            LevelManager.LevelLoaded  += HandleLevelLoaded;
        }

        private void OnDisable()
        {
            LevelManager.LevelStarted -= HandleLevelStarted;
            LevelManager.LevelLoaded  -= HandleLevelLoaded;
        }

        private void HandleLevelStarted(Level level)
        {
            GenerateGrid(level);
        }

        private void HandleLevelLoaded(Level level, PuzzleSave savedPuzzle)
        {
            LoadGrid(level, savedPuzzle);
        }

        private void LoadGrid(Level level, PuzzleSave savedPuzzle)
        {
            _gridSO = level.GridSO;

            _gridField.Initialize(_gridSO);

            var pieceConfigurations = savedPuzzle.Get2DArray();

            _gridGenerator.InitializeGrid(_gridSO, level.PuzzleData.FullSizeSprite, level.PuzzleData.IsSecret, pieceConfigurations);

            _gridInteractionController.SetRotationEnabled(level.RotationEnabled);

            _gridLoader.LoadGrid(savedPuzzle);
        }

        private void GenerateGrid(Level level)
        {
            _gridSO = level.GridSO;

            _gridField.Initialize(_gridSO);

            _gridGenerator.InitializeGrid(_gridSO, level.PuzzleData.FullSizeSprite, level.PuzzleData.IsSecret);

            _gridInteractionController.SetRotationEnabled(level.RotationEnabled);

            _scrollViewController.PopulateScrollView(_gridGenerator.GeneratedPieces, level.RotationEnabled);

            GridGenerated?.Invoke();
        }

        public List<Piece> GetScrollViewPieces()
        {
            return _scrollViewController.ContentPieces;
        }

        public List<ISnappable> GetSnappables()
        {
            return _gridInteractionController.Snappables;
        }

        public List<Piece> GetGeneratedPieces()
        {
            return _gridGenerator.GeneratedPieces;
        }

        public List<Piece> GetCollectedPieces()
        {
            return _gridInteractionController.CollectedPieces;
        }

        public List<Piece> GetNotCollectedPieces()
        {
            var notCollectedPieces = GetGeneratedPieces()
                .Except(GetCollectedPieces())
                .ToList();

            return notCollectedPieces;
        }

    }
}
