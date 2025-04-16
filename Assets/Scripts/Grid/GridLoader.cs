using System.Collections.Generic;
using UnityEngine;
using UI.GameScene;
using PuzzlePiece;
using PuzzleData.Save;
using System.Linq;
using System;

namespace Grid
{
    public class GridLoader : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        private GridInteractionController _gridInteractionController;
        private ScrollViewController _scrollViewController;
        
        public static Action GridLoaded;

        private void Awake()
        {
            _gridInteractionController = _gridManager.GridInteractionController;
            _scrollViewController = _gridManager.ScrollViewController;
        }

        public void LoadGrid(PuzzleSave savedPuzzle)
        {
            var pieces = _gridManager.GridGenerator.GeneratedPieces;

            LoadSnappablePieces(savedPuzzle.SnappableSaves, pieces);
            LoadCollectedPieces(savedPuzzle.CollectedPieceSaves, pieces);
            LoadScrollPieces(savedPuzzle.ScrollPieceSaves, pieces);

            GridLoaded?.Invoke();
        }

        private void SetupPiece(Piece piece, PieceSave pieceSave)
        {
            piece.transform.position = pieceSave.Position.ToVector3();
            piece.transform.rotation = Quaternion.Euler(pieceSave.Rotation.ToVector3());
        }

        private Piece FindAndSetupPiece(List<Piece> pieces, PieceSave pieceSave)
        {
            var piece = pieces.Find(p => p.GridPosition == pieceSave.GridPosition.ToVector2Int());
            SetupPiece(piece, pieceSave);
            return piece;
        }

        private void LoadSnappablePieces(List<SnappableSave> snappableSaves, List<Piece> pieces)
        {
            foreach (var snappableSave in snappableSaves)
            {
                var snappablePieces = snappableSave.PieceSaves
                    .Select(pieceSave => FindAndSetupPiece(pieces, pieceSave))
                    .ToList();

                if (IsSnappablePiece(snappableSave))
                {
                    _gridInteractionController.AddSnappable(snappablePieces[0]);
                }
                else
                {
                    PuzzleGroup puzzleGroup = PuzzleGroup.CreateGroup(snappablePieces);
                    _gridInteractionController.AddSnappable(puzzleGroup);
                }
            }

            _gridInteractionController.UpdateSnappablesZPositions();
        }

        private bool IsSnappablePiece(SnappableSave snappableSave)
        {
            return snappableSave.PieceSaves.Count == 1;
        }

        private void LoadCollectedPieces(List<Vector2IntS> collectedPieceSaves, List<Piece> pieces)
        {
            List <Piece> collectedPieces = new List<Piece>();

            foreach (var collectedPieceSave in collectedPieceSaves)
            {
                var piece = pieces.Find(p => p.GridPosition == collectedPieceSave.ToVector2Int());
                piece.transform.position = piece.CorrectPosition;
                collectedPieces.Add(piece);
            }

            if (collectedPieces.Count > 0)
            {
                _gridInteractionController.LoadCollectedPieces(collectedPieces);

                var collectedPuzzleGroup = PuzzleGroup.CreateGroup(collectedPieces);
                collectedPuzzleGroup.LoadAsCollected();

                _gridInteractionController.SetCollectedPiecesGroup(collectedPuzzleGroup);
            }
        }

        private void LoadScrollPieces(List<ScrollPieceSave> scrollPieceSaves, List<Piece> pieces)
        {
            List<Piece> scrollPieces = new List<Piece>();

            foreach (var scrollPieceSave in scrollPieceSaves)
            {
                var piece = pieces.Find(p => p.GridPosition == scrollPieceSave.GridPosition.ToVector2Int());
                //Debug.Log(scrollPieceSave.Rotation.ToVector3());
                piece.transform.rotation = Quaternion.Euler(scrollPieceSave.Rotation.ToVector3());
                scrollPieces.Add(piece);
            }

            _scrollViewController.PopulateScrollView(scrollPieces);
        }

    }


}
