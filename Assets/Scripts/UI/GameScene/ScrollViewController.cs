using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuzzlePiece;
using System.Linq;
using System;

namespace UI.GameScene
{
    public class ScrollViewController : MonoBehaviour
    {
        [SerializeField] private Transform _gridParent;
        [SerializeField] private RectTransform _content;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private float _pieceSize = 40;
        private float _originalPieceSize;
        private bool _isOriginalPieceSizeSet;
        private List<Piece> _contentPieces = new List<Piece>();

        public static Action StateChanged;
        public List<Piece> ContentPieces => _contentPieces; 


        private void OnEnable()
        {
            Draggable.OnItemDropped += HandleItemDropped;
            Draggable.OnItemPickedUp += HandleItemPickedUp;
        }

        private void OnDisable()
        {
            Draggable.OnItemDropped -= HandleItemDropped;
            Draggable.OnItemPickedUp -= HandleItemPickedUp;
        }

        private void HandleItemPickedUp(ISnappable snappable)
        {
            if (snappable is PuzzleGroup) return;
            _scrollRect.enabled = false;

            if (MouseOnScrollView())
            {
                RemovePieceFromScrollView(snappable.Transform);
            }
        }

        public void RemovePieceFromScrollView(Piece piece)
        {
            RemovePieceFromScrollView(piece.Transform);
        }

        private void RemovePieceFromScrollView(Transform piece)
        {
            piece.SetParent(_gridParent, true);
            
            piece.localScale = Vector3.one * _originalPieceSize;

            RectTransform rectTransform = piece as RectTransform;
            
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            _contentPieces.Remove(piece.GetComponent<Piece>());
        }

        private void HandleItemDropped(ISnappable snappable)
        {
            if (snappable is PuzzleGroup) return;

            if (MouseOnScrollView())
            {
                InsertPieceToScrollView(snappable.Transform);

                StateChanged?.Invoke();
            }
            
            _scrollRect.enabled = true;
        }

        public void PopulateScrollView(List<Piece> pieces, bool rotationEnabled=false)
        {
            List<Piece> shuffledPieces = pieces.OrderBy(x => UnityEngine.Random.Range(0, int.MaxValue)).ToList();
            //List<Piece> shuffledPieces = pieces;
            foreach (var piece in shuffledPieces)
            {
                Vector3 rotation = rotationEnabled ? GetRandomPieceRotation() : Vector3.zero;

                piece.transform.rotation = Quaternion.Euler(rotation);

                AddPieceToScrollView(piece.transform);
            }

            _contentPieces = shuffledPieces;
        }

        private Vector3 GetRandomPieceRotation()
        {
            return new Vector3(0, 0, UnityEngine.Random.Range(0, 4) * 90);
        }

        public void InsertPieceToScrollView(Transform piece)
        {
            SetOriginalPieceSize(piece);

            Vector3 position = piece.position;
    
            piece.SetParent(_content, true);

            int index = GetDropIndex(position);
            InsertPieceAtIndex(piece as RectTransform, index);

            piece.localPosition = Vector3.zero;
            piece.localScale = Vector3.one * _pieceSize;

            _contentPieces.Insert(index, piece.GetComponent<Piece>());
        }

        public void AddPieceToScrollView(Transform piece)
        {
            SetOriginalPieceSize(piece);

            piece.SetParent(_content, true);

            piece.SetAsLastSibling();

            piece.localPosition = Vector3.zero;
            piece.localScale = Vector3.one * _pieceSize;

            _contentPieces.Add(piece.GetComponent<Piece>());
        }

        private void SetOriginalPieceSize(Transform piece)
        {
            if (!_isOriginalPieceSizeSet)
            {
                _originalPieceSize = piece.transform.localScale.x;
                _isOriginalPieceSizeSet = true;
            }
        }

        public bool MouseOnScrollView()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition, Camera.main);
        }

        private int GetDropIndex(Vector3 dropScreenPosition)
        {
            Vector2 localDropPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, dropScreenPosition, null, out localDropPosition);
            
            for (int i = 0; i < _content.childCount-1; i++)
            {
                Vector2 childLocalPosition = (_content.GetChild(i) as RectTransform).anchoredPosition;
                if (localDropPosition.x < childLocalPosition.x)
                {
                    return i;
                }
            }
            return _content.childCount-1;
        }

        private void InsertPieceAtIndex(RectTransform piece, int index)
        {
            if (index >= 0 && index < _content.childCount)
            {
                piece.SetSiblingIndex(index);
            }
            else
            {
                piece.SetAsLastSibling();
            }
        }

        public bool IsInScrollView(Transform piece)
        {
            return piece.parent == _content;
        }

        # region VisiblePieces
        
        private bool IsVisibleFrom(Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
        
        private List<Piece> GetVisiblePieces(Camera camera)
        {
            return _contentPieces.Where(piece => 
                        IsVisibleFrom(piece.GetComponent<Renderer>(), camera) && piece.gameObject.activeSelf).ToList();
                    }

        public List<Piece> GetVisiblePieces()
        {
            return GetVisiblePieces(Camera.main);
        }

        # endregion

    }
}