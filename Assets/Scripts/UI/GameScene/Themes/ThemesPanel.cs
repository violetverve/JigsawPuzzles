using UnityEngine;
using DG.Tweening;
using System;

namespace UI.GameScene.Themes
{
    public class ThemesPanel : MonoBehaviour
    {
        [SerializeField] private int _hideYPosition;
        [SerializeField] private int _showYPosition;
        [SerializeField] private float _animationDuration;

        public static event Action ThemePanelShown;
        public static event Action ThemePanelHidden;

        [SerializeField] private Camera _camera;

        private void OnEnable()
        {
            SingleChoiceManager.OnThemeSelected += HandleThemeSelected;
        }

        private void OnDisable()
        {
            SingleChoiceManager.OnThemeSelected -= HandleThemeSelected;
        }

        private void HandleThemeSelected(ThemeToggle themeToggle)
        {
            _camera.backgroundColor = themeToggle.GetColor();
        }
        
        private void Awake()
        {
            SetYPosition(_hideYPosition);
        }

        private void SetYPosition(int yPosition)
        {
            var position = transform.localPosition;
            position.y = yPosition;
            transform.localPosition = position;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            GetSlideInTween().Play();

            ThemePanelShown?.Invoke();
        }

        public void Hide()
        {
            GetSlideOutTween().Play().OnComplete(HideGameObject);
        }
        
        private void HideGameObject()
        {
            gameObject.SetActive(false);
            
            ThemePanelHidden?.Invoke();
        }

        private Tween GetSlideInTween()
        {
            return transform.DOLocalMoveY(_showYPosition, _animationDuration);
        }

        private Tween GetSlideOutTween()
        {
            return transform.DOLocalMoveY(_hideYPosition, _animationDuration);
        }
    }
}