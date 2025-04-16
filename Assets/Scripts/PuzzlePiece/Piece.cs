using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace PuzzlePiece
{
    public class Piece : MonoBehaviour, ISnappable
    {
        private Vector3 _correctPosition;
        private Vector2Int _gridPosition;
        private Draggable _draggable;
        private Clickable _clickable;
        private PuzzleGroup _group;
        private MaterialBloom _materialBloom;
        private BoxCollider2D _boxCollider;
        private MeshRenderer _meshRenderer;
        private float _snapDistance;
        private float _snapRadius;
        private Vector3 _boxColliderSize;
        private bool _isEdgePiece;
        private bool _isAnimating = false;
        private float _snappingDuration = 0.05f;
        private float _rotationDuration = 0.2f;
        private float _rotationAngle = -90;
        private const int COLLECTED_Z_POSITION = 0;

        public static event Action<ISnappable> OnPieceRotated;
        public static event Action<ISnappable> OnGridSnapCompleted;
        public static event Action<ISnappable> OnPieceSnappedToGrid;
        public static event Action<ISnappable> OnCombinedWithOther;

        public Transform Transform => transform;
        public Vector3 CorrectPosition => _correctPosition;
        public Vector2Int GridPosition => _gridPosition;
        public PuzzleGroup Group => _group;
        public Draggable Draggable => _draggable;
        public bool IsAnimating => _isAnimating;
        public bool IsEdgePiece => _isEdgePiece;
        public MaterialBloom MaterialBloom => _materialBloom;
        public List<Piece> Pieces => new List<Piece> { this };
        public MeshRenderer MeshRenderer => _meshRenderer;

        private void Awake()
        {
            _boxCollider = gameObject.AddComponent<BoxCollider2D>();
            gameObject.AddComponent<RectTransform>();
            _draggable = gameObject.AddComponent<Draggable>();
            _clickable = gameObject.AddComponent<Clickable>();
            _materialBloom = gameObject.AddComponent<MaterialBloom>();
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        public void Initialize(Vector3 correctPosition, Vector2Int gridPosition, bool isEdgePiece)
        {
            _correctPosition = correctPosition;
            _gridPosition = gridPosition;
            _isEdgePiece = isEdgePiece;

            _boxColliderSize = _boxCollider.size * transform.localScale;
            _snapDistance = Mathf.Max(_boxColliderSize.x, _boxColliderSize.y);
            _snapRadius = (_snapDistance / 2) * 0.95f;
        }


        # region SnapToGrid
        public bool TrySnapToGrid() 
        {
            if (IsSnappedToGrid()) return false;
            if (!CanSnapToGrid()) return false;

            SnapToCorrectPosition();
            DestroyInteractiveComponents();

            return true;
        }

        public bool CanSnapToGrid()
        {
            return _isEdgePiece && IsWithinSnapToGridRadius() && IsInCorrectRotation();
        }

        public bool IsInCorrectRotation()
        {
            float threshold = 0.01f;
            return Mathf.Abs(transform.rotation.eulerAngles.z) < threshold;
        }

        public void SnapToCorrectPosition()
        {   
            GetSnapToCorrectPositionTween().OnComplete(FinishSnapToCorrectPosition);    
        }

        public Tween GetSnapToCorrectPositionTween()
        {
            return transform.DOMove(_correctPosition, _snappingDuration);
        }

        private void FinishSnapToCorrectPosition()
        {
            UpdateZPosition(COLLECTED_Z_POSITION);

            OnPieceSnappedToGrid?.Invoke(this);            
        }

        private bool IsWithinSnapToGridRadius()
        {
            return Vector2.Distance(transform.position, _correctPosition) < _snapRadius / 2f;
        }

        public bool IsSnappedToGrid()
        {
            if (_group != null) return _group.IsSnappedToGrid();
            return _draggable == null;
        }

        # endregion

        public void SnapToOtherPiecePosition(Piece otherPiece)
        { 
            if (IsSnappedToGrid()) return;
            if (_isAnimating) return;

            _isAnimating = true;

            GetSnapToOtherPiecePositionTween(otherPiece).OnComplete(FinishSnapToOtherPieceAnimation);
        }

        public Tween GetSnapToOtherPiecePositionTween(Piece otherPiece)
        {
            Vector3 distance = _correctPosition - otherPiece.CorrectPosition;
            
            distance = Transform.rotation * distance;

            return transform.DOMove(otherPiece.Transform.position + distance, _snappingDuration);
        }

        private void FinishSnapToOtherPieceAnimation()
        {
            _isAnimating = false;

            if (IsSnappedToGrid())
            {
                UpdateZPosition(COLLECTED_Z_POSITION);
            }

            OnCombinedWithOther?.Invoke(_group);
        }

        public ISnappable CombineWith(Piece otherPiece)
        {
            SnapToOtherPiecePosition(otherPiece);

            PuzzleGroup neighbourGroup = otherPiece.Group;

            if (neighbourGroup != null)
            {
                neighbourGroup.AddPieceToGroup(this);
                return neighbourGroup;
            } 

            return PuzzleGroup.CreateGroup(new List<Piece> { this, otherPiece });
        }

        private void DestroyInteractiveComponents()
        {
            Destroy(_draggable);
            Destroy(_clickable);

            _draggable = null;
            _clickable = null;
        }

        # region Neighbours

        public List<Piece> GetNeighbours()
        {
            List<Piece> neighbours = new List<Piece>();

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _snapRadius);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject) continue;

                AddPieceIfNeighbour(neighbours, hitCollider);
                AddGroupPiecesIfNeighbour(neighbours, hitCollider);
            }

            return neighbours;
        }

        private void AddPieceIfNeighbour(List<Piece> neighbours, Collider2D hitCollider)
        {
            Piece piece = hitCollider.transform.GetComponent<Piece>();
            if (piece == null || piece == this) return;

            if (IsNeighbourToCombine(piece))
            {
                neighbours.Add(piece);
            }
        }

        private void AddGroupPiecesIfNeighbour(List<Piece> neighbours, Collider2D hitCollider)
        {
            PuzzleGroup group = hitCollider.GetComponent<PuzzleGroup>();
            if (group != null)
            {
                foreach (Transform child in group.transform)
                {
                    Collider2D childCollider = child.GetComponent<Collider2D>();
                    if (childCollider != null)
                    {
                        AddPieceIfNeighbour(neighbours, childCollider);
                    }
                }
            }
        }

        private bool IsNeighbourToCombine(Piece piece)
        {
            return IsNeighbour(piece.GridPosition) && IsInCombinableRange(piece) && IsAlignedWithGrid(piece);
        }

        public Piece GetNeighbourPiece()
        {
            List<Piece> neighbours = GetNeighbours();
            return neighbours.Count > 0 ? neighbours[0] : null;
        }

        public bool IsNeighbour(Vector2Int otherGridPosition)
        {
            return Mathf.Abs(_gridPosition.x - otherGridPosition.x) == 1 && _gridPosition.y == otherGridPosition.y ||
                   Mathf.Abs(_gridPosition.y - otherGridPosition.y) == 1 && _gridPosition.x == otherGridPosition.x;
        }

        # endregion
        
        # region CombiningConditions

        private bool IsInCombinableRange(Piece piece)
        {
            float dist = Vector2.Distance(transform.position, piece.Transform.position);
            return dist < _snapDistance && dist > _snapDistance / 2;
        }

        private bool IsAlignedWithGrid(Piece piece)
        {
            Vector2 gridDistance = _gridPosition - piece.GridPosition;
            Vector2 realDistance = transform.position - piece.transform.position;

            float rotationAngle = piece.transform.eulerAngles.z * Mathf.Deg2Rad;

            Vector2 rotatedRealDistance = new Vector2(
                realDistance.x * Mathf.Cos(rotationAngle) + realDistance.y * Mathf.Sin(rotationAngle),
                -realDistance.x * Mathf.Sin(rotationAngle) + realDistance.y * Mathf.Cos(rotationAngle)
            );

            bool signsMatch = Mathf.Sign(gridDistance.x) == Mathf.Sign(rotatedRealDistance.x) &&
                              Mathf.Sign(gridDistance.y) == Mathf.Sign(rotatedRealDistance.y);

            float tolerance = 0.2f;
            bool isPerpendicular = Mathf.Abs(rotatedRealDistance.x) < tolerance || Mathf.Abs(rotatedRealDistance.y) < tolerance;

            return signsMatch && isPerpendicular;
        }


        # endregion

        public void ClampToGrid(GetClampedPositionDelegate getClampedPosition, bool mouseOnScrollView)
        {
            if (mouseOnScrollView) return;
            
            Vector3 pieceCenter = _boxCollider.bounds.center;

            Vector3 pieceSize =  new Vector2(_boxCollider.size.x, _boxCollider.size.y) * transform.localScale;

            Vector3 clampedPosition = getClampedPosition(pieceCenter, pieceSize);
            
            Vector3 offset = pieceCenter - transform.position;

            if (transform.position == clampedPosition - offset)
            {
                OnGridSnapCompleted?.Invoke(this);
                return;
            }
            
            transform.DOMove(clampedPosition - offset, _snappingDuration).OnComplete(FinishClampToGrid);
        }

        private void FinishClampToGrid()
        {
            OnGridSnapCompleted?.Invoke(this);
        }

        public void AddToCollectedPieces(List<Piece> collectedPieces)
        {
            collectedPieces.Add(this);
        }

        public void SetGroup(PuzzleGroup group)
        {
            _group = group;
            Transform.SetParent(group.transform, true);
        }

        public void SetupForGroup()
        {
            _boxCollider.usedByComposite = true;
            DestroyInteractiveComponents();
        }

        public void UpdateZPosition(int zPosition)
        {
            Vector3 position = transform.position;
            position.z = zPosition;
            transform.position = position;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _snapRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_boxCollider.bounds.center, _boxColliderSize);
        }

        # region Rotation
        public void Rotate(Vector3 mouseWorldPos)
        {
            if (_isAnimating) return;
            
            _isAnimating = true;
            
            transform.DORotate(transform.eulerAngles + new Vector3(0, 0, _rotationAngle), _rotationDuration)
                     .OnComplete(FinishRotation);
        }

        private void FinishRotation()
        {
            _isAnimating = false;

            OnPieceRotated?.Invoke(this);
        }

        public bool HaveSameRotation(Piece piece)
        {
            float threshold = 0.01f;
        
            float rotationDifference = Mathf.Abs(transform.rotation.eulerAngles.z - piece.transform.rotation.eulerAngles.z);

            return rotationDifference < threshold;
        }
        
        public bool IsPointOnPiece(Vector3 point)
        {
            Vector2 mousePos = new Vector2(point.x, point.y);
        
            Vector2 colliderPos = _boxCollider.transform.position;
            Vector2 colliderSize = _boxColliderSize;
            Vector2 min = colliderPos - colliderSize / 2;
            Vector2 max = colliderPos + colliderSize / 2;
        
            return IsWithinRange(mousePos.x, min.x, max.x) && IsWithinRange(mousePos.y, min.y, max.y);
        }
        
        private bool IsWithinRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        # endregion

        # region Animation
        public void StartMaterialAnimation(float bloomIntensityCoefficient = 1)
        {
            _materialBloom.AnimateMaterial(bloomIntensityCoefficient);
        }

        public void AnimateToCorrectPosition(float duration, int zPosition)
        {
            UpdateZPosition(zPosition);

            Vector3 offset = new Vector3(0.005f, 0.005f, 0);

            Vector3 correctPosition = _correctPosition + offset;
            _correctPosition.z = zPosition;
            
            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(correctPosition, duration));
            sequence.Join(transform.DORotate(Vector3.zero, duration));

            sequence.OnComplete(InvokeItemDropped);
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