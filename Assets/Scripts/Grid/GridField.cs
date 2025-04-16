using UnityEngine;
using PuzzlePiece;
using UI.GameScene;

namespace Grid
{
    public class GridField : MonoBehaviour
    {
        [SerializeField] private ScrollViewController _scrollViewController;
        private RectTransform _rectTransform;
        private float _width;
        private GridSO _gridSO;
        private float _cellSize;
        public float Width => _width;
        private Vector3 _startCorner;
        public Vector3 StartCorner => _startCorner;
        public float CellSize => _cellSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(GridSO gridSO)
        {
            _gridSO = gridSO;

            _startCorner = GetStartCorner();
            _width = CalculateWidth();
            _cellSize = CalculateCellSize();
        }

        private float CalculateCellSize()
        {
            return _width / _gridSO.Width;
        }

        public Vector3 GetStartCorner()
        {
            Vector3[] worldCorners = new Vector3[4];
            _rectTransform.GetWorldCorners(worldCorners);

            return worldCorners[0];
        }

        public float CalculateWidth()
        {
            Vector3[] worldCorners = new Vector3[4];
            _rectTransform.GetWorldCorners(worldCorners);
            
            return Vector3.Distance(worldCorners[0], worldCorners[3]);
        }

        private void OnEnable()
        {
            Draggable.OnItemDropped += HandleItemDropped;
            Piece.OnPieceRotated += HandleISnapableRotated;
            PuzzleGroup.OnGroupRotated += HandleISnapableRotated;
        }

        private void OnDisable()
        {
            Draggable.OnItemDropped -= HandleItemDropped;
            Piece.OnPieceRotated -= HandleISnapableRotated;
            PuzzleGroup.OnGroupRotated -= HandleISnapableRotated;
        }

        private void HandleISnapableRotated(ISnappable snappable)
        {
            ClampToGrid(snappable);
        }

        private void HandleItemDropped(ISnappable snappable)
        {
            ClampToGrid(snappable);
        }

        # region GridClamping
        private void ClampToGrid(ISnappable snappable)
        {
            snappable.ClampToGrid(GetClampedPosition, _scrollViewController.MouseOnScrollView());
        }
        
        public Vector3 GetClampedPosition(Vector3 position, Vector2 size)
        {
            float halfObjectWidth = size.x / 2;
            float halfObjectHeight = size.y / 2;

            float startX = transform.position.x - (_gridSO.Width * _cellSize) / 2 + halfObjectWidth;
            float startY = transform.position.y - (_gridSO.Height * _cellSize) / 2 + halfObjectHeight;
            float endX = transform.position.x + (_gridSO.Width * _cellSize) / 2 - halfObjectWidth;
            float endY = transform.position.y + (_gridSO.Height * _cellSize) / 2 - halfObjectHeight;

            Vector3 clampedPosition = position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, startX, endX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, startY, endY);

            return clampedPosition;
        }

        # endregion

        void OnDrawGizmos()
        {
            if (_gridSO != null)
            {
                float startX = _startCorner.x;
                float startY = _startCorner.y;

                float endX = _startCorner.x + _width;
                float endY = _startCorner.y + _width;

                Gizmos.color = Color.gray;

                // Drawing the vertical lines
                for (int i = 0; i <= _gridSO.Width; i++)
                {
                    float lineX = startX + (i * _cellSize);
                    Gizmos.DrawLine(
                        new Vector3(lineX, startY, 0),
                        new Vector3(lineX, endY, 0));
                }

                // Drawing the horizontal lines
                for (int j = 0; j <= _gridSO.Height; j++)
                {
                    float lineY = startY + (j * _cellSize);
                    Gizmos.DrawLine(
                        new Vector3(startX, lineY, 0),
                        new Vector3(endX, lineY, 0));
                }
            }
        }

    }
}
