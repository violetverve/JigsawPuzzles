using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using PuzzlePiece;
using Grid;
using System.Linq;
using System.Collections.Generic;
using GameManagement;

namespace UI.GameScene
{
    public class ToggleEdgePieces : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private ScrollViewController _scrollViewController;
        [SerializeField] private ScrollViewAnimator _scrollViewAnimator;
        [SerializeField] private Toggle _toggleButton;
        [SerializeField] private float _duration = 0.5f;
        private Vector3 _initialScale;

        private void OnEnable()
        {
            ProgressManager.EdgesCollectedLoaded += HandleEdgesCollectedLoaded;
            ProgressManager.EdgesCollected += HandleEdgesCollected;
        }

        private void OnDisable()
        {
            ProgressManager.EdgesCollectedLoaded -= HandleEdgesCollectedLoaded;
            ProgressManager.EdgesCollected -= HandleEdgesCollected;
        }

        private void HandleEdgesCollectedLoaded(bool edgesCollected)
        {
            if (edgesCollected)
            {
                HandleEdgesCollected();
            }
        }

        private void HandleEdgesCollected()
        {
            _toggleButton.isOn = false;
            _toggleButton.interactable = false;
        }

        public void Toggle()
        {
            var gridPieces = _gridManager.GetSnappables()
                .Select(snappable => snappable as Piece)
                .Where(piece => piece != null)
                .ToList();

            if (_initialScale == Vector3.zero){
                if (gridPieces.Count > 0)
                    _initialScale = gridPieces[0].Transform.localScale;
            }

            ToggleNonEdgePieces(gridPieces, true);

            Tween hideScrollView = _scrollViewAnimator.GetHideTween(_duration/2);

            hideScrollView.OnComplete(() =>
            {
                ToggleNonEdgePieces(_scrollViewController.ContentPieces, false);
                _scrollViewAnimator.Show(_duration/2);
            });
        }

        private void ToggleNonEdgePieces(List<Piece> pieces, bool animate)
        {
            pieces.ForEach(piece =>
            {
                if (!piece.IsEdgePiece)
                {
                    if (animate)
                    {
                        AnimatePieceToggle(piece);
                    }
                    else
                    {
                        piece.gameObject.SetActive(!piece.gameObject.activeSelf);
                    }
                }
            });
        }

        private void AnimatePieceToggle(Piece piece)
        {
            if (piece.IsEdgePiece) return;

            bool active = piece.gameObject.activeSelf;

            if (active)
            {
                piece.gameObject.transform.DOScale(_initialScale * 0.5f, _duration/2)
                    .OnComplete(() => piece.gameObject.SetActive(false));
            }
            else
            {
                piece.gameObject.SetActive(true);
                piece.gameObject.transform.DOScale(_initialScale, _duration/2);
            }

        }

    }

}
