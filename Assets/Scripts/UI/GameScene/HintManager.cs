using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Grid;
using PuzzlePiece;
using System;
using System.Linq;
using Player;

namespace UI.GameScene
{
    public class HintManager : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private ScrollViewController _scrollViewController;
        [SerializeField] private TextMeshProUGUI _hintsNumberText;
        [SerializeField] private Button _hintButton;

        private const int TOP_LAYER = -9;
        private int _hintsNumber = 30;
        private float _hintDuration = 0.7f;
        private int _visiblePieces = 4;

        private void Awake()
        {
            if (PlayerData.Instance != null)
            {
                _hintsNumber = PlayerData.Instance.Hints;
            }

            SetNumberHintsText(_hintsNumber);
        }

        private void UseHint()
        {
            if (_hintsNumber <= 0) return;

            bool hintUsed = TryUseHint();

            if (hintUsed)
            {
                _hintsNumber--;
                SetNumberHintsText(_hintsNumber);
               
               if (PlayerData.Instance != null)
                {
                    PlayerData.Instance.UseHint();
                }
            }
        }

        private bool TryUseHint()
        {
            // snappable pieces that have neighbours collected or are edge pieces
            var snappables = _gridManager.GetSnappables();
            
            var hintSnappable = GetHintSnappable(snappables);
            if (hintSnappable != null)
            {
                hintSnappable.AnimateToCorrectPosition(_hintDuration, TOP_LAYER);
                PauseHintButton();
                return true;
            }

            // visible scrollview pieces that have neighbours collected or are edge pieces
            var visibleScrollViewPieces = _scrollViewController.GetVisiblePieces();

            var hintPiece = GetHintPiece(visibleScrollViewPieces);
            if (hintPiece != null)
            {
                _scrollViewController.RemovePieceFromScrollView(hintPiece);

                hintPiece.AnimateToCorrectPosition(_hintDuration, TOP_LAYER);
                PauseHintButton();
                return true;
            }
            
            // any snappable
            if (TryUseHintOnSnappables(snappables)) return true;

            // any visible scrollview piece
            if (TryUseHintOnPieces(visibleScrollViewPieces)) return true;

            return false;
        }
        
        # region HintItemSelection

        private bool HasNeighbourCollected(Piece piece)
        {
            return _gridManager.CollectedPieces.Any(collectedPiece => 
            collectedPiece.IsNeighbour(piece.GridPosition));
        }
        
        private Piece GetHintPiece(List<Piece> pieces)
        {
            return pieces.FirstOrDefault(piece => (HasNeighbourCollected(piece) || piece.IsEdgePiece) && piece.gameObject.activeSelf);
        }

        private ISnappable GetHintSnappable(List<ISnappable> snappables)
        {
            return snappables.FirstOrDefault(snappable => snappable.Pieces.Any(piece => 
            HasNeighbourCollected(piece) || piece.IsEdgePiece));
        }

        # endregion 

        private bool TryUseHintOnPieces(List<Piece> pieces)
        {
            if (pieces.Count <= 0) return false;

            int maxIndex = Math.Min(_visiblePieces, pieces.Count);
            var randomPiece = pieces[UnityEngine.Random.Range(0, pieces.Count)];

            _scrollViewController.RemovePieceFromScrollView(randomPiece);

            randomPiece.AnimateToCorrectPosition(_hintDuration, TOP_LAYER);
            
            PauseHintButton();
            return true;
        }

        private bool TryUseHintOnSnappables(List<ISnappable> snappables)
        {
            if (snappables.Count <= 0) return false;

            var randomSnappable = snappables[UnityEngine.Random.Range(0, snappables.Count)];
            randomSnappable.AnimateToCorrectPosition(_hintDuration, TOP_LAYER);

            PauseHintButton();
            return true;
        }

        private void PauseHintButton()
        {
            _hintButton.interactable = false;

            StartCoroutine(ReenableHintButtonAfterDuration(_hintDuration));
        }

        private IEnumerator ReenableHintButtonAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            _hintButton.interactable = true;
        }

        private void SetNumberHintsText(int number)
        {
            _hintsNumberText.text = number.ToString();
        }
        
    }
}