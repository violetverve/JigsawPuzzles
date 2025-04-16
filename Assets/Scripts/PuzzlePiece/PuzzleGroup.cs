using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

namespace PuzzlePiece
{
    public class PuzzleGroup : MonoBehaviour, ISnappable
    {
        private const string PUZZLE_GROUP = "PuzzleGroup";
        private Draggable _draggable;
        private Clickable _clickable;
        private CompositeCollider2D _compositeCollider;
        private List<Piece> _pieces = new List<Piece>();
        private List<Piece> _lastPieces = new List<Piece>();
        private bool _isAnimating = false;
        private const int COLLECTED_Z_POSITION = 0;
        private float _snappingDuration = 0.05f;
        private float _rotationDuration = 0.2f;
        private float _rotationAngle = -90;

        public static event Action<ISnappable> OnGroupRotated;
        public static event Action<ISnappable> OnGridSnapCompleted;
        public static event Action<ISnappable> OnGroupSnappedToGrid;
        public static event Action<ISnappable> OnCombinedWithOther;

        public Transform Transform => transform;
        public List<Piece> Pieces => _pieces;
        public Draggable Draggable => _draggable;
        public bool IsAnimating => _isAnimating;

        public void InitializeGroup(List<Piece> pieces)
        {
            _draggable = gameObject.AddComponent<Draggable>();
            _clickable = gameObject.AddComponent<Clickable>();

            List<Piece> piecesToAdd = pieces;

            foreach (Piece piece in piecesToAdd)
            {
                AddPieceToGroup(piece);
            }

            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CompositeCollider2D compositeCollider = gameObject.AddComponent<CompositeCollider2D>();
            compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
            compositeCollider.generationType = CompositeCollider2D.GenerationType.Synchronous;

            _compositeCollider = compositeCollider;
        }

        public static PuzzleGroup CreateGroup(List<Piece> pieces)
        {
            Vector3 centerPoint = GetCenterPoint(pieces);
            
            Transform parent = pieces[0].transform.parent;

            GameObject groupObject = new GameObject(PUZZLE_GROUP);
            groupObject.transform.parent = parent;
            groupObject.transform.position = centerPoint;

            PuzzleGroup group = groupObject.AddComponent<PuzzleGroup>();
            group.InitializeGroup(pieces);

            return group;
        }
        
        private static Vector3 GetCenterPoint(List<Piece> _pieces)
        {
            return _pieces.Aggregate(Vector3.zero, (sum, piece) => sum + piece.transform.position) / _pieces.Count;
        }

        # region SnapToGrid
        public bool TrySnapToGrid()
        {
            if (IsSnappedToGrid()) return false;

            bool snap = _pieces.Any(piece => piece.CanSnapToGrid()); 

            if (snap)
            {
                Sequence sequence = DOTween.Sequence();

                foreach (Piece piece in _pieces)
                {
                    Tween moveTween = piece.GetSnapToCorrectPositionTween();
                    sequence.Join(moveTween);
                }

                DestroyInteractiveComponents();

                sequence.OnComplete(FinishSnapToCorrectPosition);
            }

            return snap;
        }


        private void FinishSnapToCorrectPosition()
        {
            UpdateZPosition(COLLECTED_Z_POSITION);

            OnGroupSnappedToGrid?.Invoke(this);
        }

        public bool IsSnappedToGrid()
        {
            return _draggable == null;
        }

        private void DestroyInteractiveComponents()
        {
            Destroy(_draggable);
            Destroy(_clickable);

            _draggable = null;
            _clickable = null;
        }

        # endregion

        public ISnappable CombineWith(Piece otherPiece)
        {
            SnapGroupPositionToOtherPiece(otherPiece);

            PuzzleGroup neighbourGroup = otherPiece.Group;

            if (neighbourGroup != null)
            {
                MergeGroup(neighbourGroup);
            }
            else
            {
                AddPieceToGroup(otherPiece);
            }

            return this;
        }

        public Piece GetNeighbourPiece()
        {
            foreach (Piece piece in _pieces)
            {
                List<Piece> neighbours = piece.GetNeighbours();

                foreach (Piece neighbour in neighbours)
                {
                    if (!IsTheSameGroup(neighbour.Group))
                    {
                        return neighbour;
                    }
                }
            }
            return null;
        }

        private void SnapGroupPositionToOtherPiece(Piece referencePiece)
        {
            if (IsSnappedToGrid()) return;

            if (_isAnimating) return;

            _isAnimating = true;

            Sequence sequence = DOTween.Sequence();

            foreach (Piece piece in _pieces)
            {
                Tween moveTween = piece.GetSnapToOtherPiecePositionTween(referencePiece);   
                sequence.Join(moveTween);
            }

            sequence.OnComplete(FinishSnapGroupAnimation);
        }

        private void FinishSnapGroupAnimation()
        {
            _isAnimating = false;

            if (IsSnappedToGrid())
            {
                UpdateZPosition(COLLECTED_Z_POSITION);
            }

            OnCombinedWithOther?.Invoke(this);
        }

        public void AddPieceToGroup(Piece piece)
        {
            bool pieceInteractable = piece.Draggable != null;

            _pieces.Add(piece);
        
            if (!pieceInteractable)
            {
                DestroyInteractiveComponents();
                UpdateZPosition(COLLECTED_Z_POSITION);
            }

            piece.SetupForGroup();
            piece.SetGroup(this);
        }

        private bool IsTheSameGroup(PuzzleGroup group)
        {
            return group == this;
        } 


        private void MergeGroup(PuzzleGroup otherGroup)
        {
            bool otherGroupsInteractable = otherGroup.Draggable != null;

            UpdatePiecesGroup(otherGroup.Pieces);

            if (!otherGroupsInteractable)
            {
                DestroyInteractiveComponents();
                UpdateZPosition(COLLECTED_Z_POSITION);
            }
        
            Destroy(otherGroup.gameObject);
        }

        private void UpdatePiecesGroup(List<Piece> pieces)
        {    
            pieces.ForEach(piece => piece.SetGroup(this));
            _pieces.AddRange(pieces);
        }

        public void ClampToGrid(GetClampedPositionDelegate getClampedPosition, bool mouseOnScrollView)
        {
            Bounds groupBounds = _compositeCollider.bounds;

            Vector2 groupSize = new Vector2(groupBounds.size.x, groupBounds.size.y);

            Vector3 clampedPosition = getClampedPosition(groupBounds.center, groupSize);

            Vector3 offset = groupBounds.center - transform.position;

            if (clampedPosition - offset == transform.position) {
                OnGridSnapCompleted?.Invoke(this);
                return;
            } 
            
            transform.DOMove(clampedPosition - offset, _snappingDuration)
                     .OnComplete(FinishClampToGrid);
        }

        private void FinishClampToGrid()
        {
            OnGridSnapCompleted?.Invoke(this);
        }

        public void AddToCollectedPieces(List<Piece> collectedPieces)
        {
            _pieces.ForEach(piece => collectedPieces.Add(piece));
        }

        public void UpdateZPosition(int zPosition = COLLECTED_Z_POSITION)
        {
            Vector3 position = transform.position;
            position.z = zPosition;
            transform.position = position;

            _pieces.ForEach(piece => piece.UpdateZPosition(zPosition));
        }


        # region Rotation

        public void Rotate(Vector3 mousePosition)
        {
            if (_isAnimating) return;

            var piece = GetPieceAtMousePosition(mousePosition);

            _isAnimating = true;

            var sequence = GetRotationSequence(piece, _rotationAngle, _rotationDuration);

            sequence.OnComplete(FinishRotation);
        }

        public Sequence GetRotationSequence(Piece piece, float rotationAngle, float rotationDuration)
        {
            Sequence sequence = DOTween.Sequence();

            foreach (Transform child in transform)
            {
                Vector3 newPosition = RotatePointAroundPivot(child.position, piece.transform.position, rotationAngle);

                int pathPoints = 5;
                Vector3[] path = GetCircularPath(child.position, piece.transform.position, rotationAngle, pathPoints);

                Tween moveTween = child.DOPath(path, rotationDuration, PathType.CatmullRom);
                Tween rotateTween = child.DORotate(child.eulerAngles + new Vector3(0, 0, rotationAngle), rotationDuration);

                sequence.Insert(0, moveTween);
                sequence.Insert(0, rotateTween);
            }

            return sequence;
        }

        private void FinishRotation()
        {
            _isAnimating = false;

            OnGroupRotated?.Invoke(this);
        }

        private Vector3[] GetCircularPath(Vector3 startPoint, Vector3 pivot, float angle, int segments)
        {
            Vector3[] path = new Vector3[segments + 1];
            float angleStep = angle / segments;

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = i * angleStep;
                path[i] = RotatePointAroundPivot(startPoint, pivot, currentAngle);
            }

            return path;
        }

        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
        {
            Vector3 relativePosition = point - pivot;
            relativePosition = Quaternion.Euler(0, 0, angle) * relativePosition;
            return pivot + relativePosition;
        }

        private Piece GetPieceAtMousePosition(Vector3 mousePosition)
        {
            return _pieces.FirstOrDefault(piece => piece.IsPointOnPiece(mousePosition));
        }

        public bool HaveSameRotation(Piece piece)
        {
            return piece.HaveSameRotation(_pieces[0]);
        }

        # endregion

        # region Animation  
        
        public void AnimateToCorrectPosition(float duration, int zPosition)
        {
            Piece pivotPiece = GetCenterPiece();
            UpdateZPosition(zPosition);
            
            Sequence sequence = DOTween.Sequence();

            if (!pivotPiece.IsInCorrectRotation())
            {
                float angle = -pivotPiece.transform.rotation.eulerAngles.z;

                // for -270 to smooth rotation
                angle = angle < -180 ? angle + 360 : angle;

                sequence.Append(GetRotationSequence(pivotPiece, angle, duration/2));
            }

            Sequence moveSequence = DOTween.Sequence();

            foreach (Piece piece in _pieces)
            {
                Vector3 correctPosition = piece.CorrectPosition;
                correctPosition.z = zPosition;

                Tween moveTween = piece.transform.DOMove(correctPosition, duration);
                moveSequence.Join(moveTween);
            }

            sequence.Append(moveSequence);

            sequence.OnComplete(InvokeItemDropped);
        }

        private Piece GetCenterPiece()
        {
            Vector3 averagePosition = GetCenterPoint(_pieces);
        
            return _pieces.OrderBy(piece => Vector3.Distance(piece.CorrectPosition, averagePosition)).First();
        }

        # endregion 

        private void InvokeItemDropped()
        {
            _draggable.InvokeItemDropped();
        }

        public void LoadAsCollected()
        {
            DestroyInteractiveComponents();
        }

    }
}
