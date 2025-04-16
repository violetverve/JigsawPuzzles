using System.Collections.Generic;
using UnityEngine;
using PuzzlePiece;
using System;
using UI.GameScene;
using System.Linq;

namespace Grid
{
    public class GridInteractionController : MonoBehaviour
    {
        [SerializeField] private ScrollViewController _scrollViewController;
        private List<ISnappable> _snappables = new List<ISnappable>();
        private List<Piece> _collectedPieces = new List<Piece>();
        private bool _rotationEnabled;
        private List<Piece> _corePieces = new List<Piece>();

        public static event Action<int, int> OnProgressUpdate;
        public static event Action<List<Piece>, List<Piece>> PiecesCollected;
        public static event Action OnStateChanged; // used for saving
        public List<Piece> CollectedPieces => _collectedPieces;
        public List<ISnappable> Snappables => _snappables;
        public List<Piece> CorePieces => _corePieces;
        private PuzzleGroup _collectedPiecesGroup;


        private void OnEnable()
        {
            Draggable.OnItemPickedUp += HandleItemPickedUp;
            Clickable.OnItemClicked += HandleItemClicked;

            Piece.OnGridSnapCompleted += HandleItemDropped;
            PuzzleGroup.OnGridSnapCompleted += HandleItemDropped;

            Piece.OnPieceSnappedToGrid += HandleSnapedToGrid;
            PuzzleGroup.OnGroupSnappedToGrid += HandleSnapedToGrid;

            Piece.OnCombinedWithOther += HandleCombinedWithOther;
            PuzzleGroup.OnCombinedWithOther += HandleCombinedWithOther;
        }

        private void OnDisable()
        {
            Draggable.OnItemPickedUp -= HandleItemPickedUp;
            Clickable.OnItemClicked -= HandleItemClicked;

            Piece.OnGridSnapCompleted -= HandleItemDropped;
            PuzzleGroup.OnGridSnapCompleted -= HandleItemDropped;

            Piece.OnPieceSnappedToGrid -= HandleSnapedToGrid;
            PuzzleGroup.OnGroupSnappedToGrid -= HandleSnapedToGrid;

            Piece.OnCombinedWithOther -= HandleCombinedWithOther;
            PuzzleGroup.OnCombinedWithOther -= HandleCombinedWithOther;
        }

        private void HandleCombinedWithOther(ISnappable snappable)
        {
            UpdateCompletedPiecesGroup(snappable);

            PiecesCollected?.Invoke(_corePieces, snappable.Pieces);

            OnStateChanged?.Invoke();
        }

        public void AddSnappable(ISnappable snappable)
        {
            _snappables.Add(snappable);
        }

        private void HandleSnapedToGrid(ISnappable snappable)
        {
            _corePieces = new List<Piece>(snappable.Pieces);

            UpdateCompletedPieces(snappable.Pieces);

            UpdateCompletedPiecesGroup(snappable);

            PiecesCollected?.Invoke(_corePieces, _collectedPiecesGroup?.Pieces);
   
            OnStateChanged?.Invoke();
        }

        private void UpdateCompletedPiecesGroup(ISnappable snappable)
        {
            if (snappable.IsSnappedToGrid())
            {
                if (_collectedPiecesGroup == null)
                {
                    _collectedPiecesGroup = PuzzleGroup.CreateGroup(snappable.Pieces);
                    _collectedPiecesGroup.Transform.SetParent(transform);
                }
                else if (snappable.Transform != _collectedPiecesGroup.Transform)
                {
                    _collectedPiecesGroup.CombineWith(snappable.Pieces[0]);
                }
            }
        }

        public void LoadCollectedPieces(List<Piece> pieces)
        {
            _collectedPieces = pieces;
        }

        public int GetCollectedEdgePiecesCount()
        {
            return GetEdgePieces(_collectedPieces).Count;
        }

        private void UpdateCompletedPieces(List<Piece> pieces)
        {
            _collectedPieces.AddRange(pieces);
            var edgePieces = GetEdgePieces(_collectedPieces);

            //Debug.Log("Collected Pieces: " + _collectedPieces.Count + " Edge Pieces: " + edgePieces.Count);
            OnProgressUpdate?.Invoke(_collectedPieces.Count, GetEdgePieces(_collectedPieces).Count);
        }
        
        private List<Piece> GetEdgePieces(List<Piece> pieces)
        {
            return pieces.Where(piece => piece.IsEdgePiece).ToList();
        }

        public void SetRotationEnabled(bool rotationEnabled)
        {
            _rotationEnabled = rotationEnabled;
        }

        private void HandleItemClicked(ISnappable snappable, Vector3 mousePosition)
        {
            if (!_rotationEnabled) return;
            if (snappable is Piece piece)
            {
                if (IsInScrollView(piece)) return;
            }

            snappable.Rotate(mousePosition);

            OnStateChanged?.Invoke();
        }

        private void HandleItemPickedUp(ISnappable snappable)
        {
            MoveToTop(snappable);

            _snappables.Remove(snappable);
        }

        private void HandleItemDropped(ISnappable snappable)
        {   
            if (!_snappables.Contains(snappable))
            {
                _snappables.Add(snappable);
            }

            UpdateSnappablesZPositions();

            if (TrySnapToGrid(snappable)) return;

            _corePieces = new List<Piece>(snappable.Pieces);

            if (SingleTryCombineWithOther(snappable) != null && snappable.IsSnappedToGrid())
            {
                UpdateCompletedPieces(_corePieces);
            }

            OnStateChanged?.Invoke();
        }

        private void MoveToTop(ISnappable snappable)
        {
            snappable.UpdateZPosition(-_snappables.Count - 1);
        }

        public void UpdateSnappablesZPositions()
        {
            for (int i = 0; i < _snappables.Count; i++)
            {
                _snappables[i].UpdateZPosition(-i - 1);
            }
        }

        private bool TrySnapToGrid(ISnappable snappable)
        {
            if (!snappable.TrySnapToGrid()) return false;
            
            _snappables.Remove(snappable);

            return true;
        }

        private ISnappable SingleTryCombineWithOther(ISnappable snappable)
        {
            Piece neighbourPiece = snappable.GetNeighbourPiece();

            if (!CanSnap(neighbourPiece) || !snappable.HaveSameRotation(neighbourPiece)){
                return null;
            }

            _snappables.Remove(snappable);
            _snappables.Remove(neighbourPiece);
            _snappables.Remove(neighbourPiece.Group);

            ISnappable combined = snappable.CombineWith(neighbourPiece);

            if (!combined.IsSnappedToGrid())
            {
                _snappables.Add(combined);
            }

            return combined;
        }
        
        private bool CanSnap(Piece piece)
        {
            return piece != null && !IsInScrollView(piece);
        }

        private bool IsInScrollView(Piece piece)
        {
            return _scrollViewController.IsInScrollView(piece.Transform);
        }

        public void SetCollectedPiecesGroup(PuzzleGroup collectedPiecesGroup)
        {
            if (_collectedPiecesGroup == null)
            {
                _collectedPiecesGroup = collectedPiecesGroup;
            }
        }

    }
}
