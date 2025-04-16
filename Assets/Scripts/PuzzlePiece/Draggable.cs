using UnityEngine;
using System;

namespace PuzzlePiece
{
    public class Draggable : MonoBehaviour
    {
        public static event Action<ISnappable> OnItemDropped;
        public static event Action<ISnappable> OnItemPickedUp;


        private Camera _mainCamera;
        private bool _isDragging;
        private Vector3 _offset;
        private ISnappable _snappable;
        private Vector3 _initialPosition;
        private float _distanceTolerance = 0.01f;


        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            _snappable = gameObject.GetComponent<ISnappable>();
        }

        private void OnMouseDown()
        {
            _isDragging = true;
            _offset = transform.position - GetMouseWorldPos();
            _initialPosition = transform.position;

            OnItemPickedUp?.Invoke(_snappable);
        }

        private void OnMouseUp()
        {
            _isDragging = false;

            if (Vector3.Distance(_initialPosition, transform.position) > _distanceTolerance)
            {
                OnItemDropped?.Invoke(_snappable);
            }
        }

        private void OnMouseDrag()
        {
            if (_isDragging)
            {
                transform.position = GetMouseWorldPos() + _offset;
            }
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = _mainCamera.WorldToScreenPoint(transform.position).z;
            return _mainCamera.ScreenToWorldPoint(mousePoint);
        }

        public void InvokeItemDropped()
        {
            OnItemDropped?.Invoke(_snappable);
        }
    }
}