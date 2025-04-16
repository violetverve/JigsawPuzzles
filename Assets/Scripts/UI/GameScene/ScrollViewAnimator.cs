using UnityEngine;
using DG.Tweening;
using UI.GameScene.Themes;

namespace UI.GameScene
{
    public class ScrollViewAnimator : MonoBehaviour
    {
        private bool _isShown = true;
        private Vector3 _hiddenPosition;
        private Vector3 _shownPosition;
        [SerializeField] private float _yDistance = 150;
        [SerializeField] private float _duration = 0.1f;

        private void OnEnable()
        {
            ThemesPanel.ThemePanelShown += OnThemesPanelShown;
            ThemesPanel.ThemePanelHidden += OnThemesPanelHidden;
        }

        private void OnDisable()
        {
            ThemesPanel.ThemePanelShown -= OnThemesPanelShown;
            ThemesPanel.ThemePanelHidden -= OnThemesPanelHidden;
        }

        private void OnThemesPanelShown()
        {
            Hide(_duration);
        }

        private void OnThemesPanelHidden()
        {
            Show(_duration);
        }

        private void Start()
        {
            _shownPosition = transform.localPosition;

            _hiddenPosition = _shownPosition - new Vector3(0, _yDistance, 0);
        }

        public void Show(float duration)
        {
            GetShowTween(duration).Play();
        }

        public void Hide(float duration)
        {
            GetHideTween(duration).Play();
        }

        public Tween GetShowTween(float duration)
        {
            return transform.DOLocalMove(_shownPosition, duration);
        }

        public Tween GetHideTween(float duration)
        {
            return transform.DOLocalMove(_hiddenPosition, duration);
        }

        public void Toggle(float duration)
        {
            if (_isShown)
            {
                Hide(duration);
            }
            else
            {
                Show(duration);
            }

            _isShown = !_isShown;
        }

        public void Toggle(bool isOn, float duration)
        {
            transform.DOKill();
            transform.DOLocalMove(isOn ? _shownPosition : _hiddenPosition, duration);
        }
    }
}