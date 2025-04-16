using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace UI.GameScene
{
    public class TogglePanels : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _panel;
        [SerializeField] private ScrollViewAnimator _scrollViewAnimator;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private float _fadeDuration = 0.5f;

        private void Awake()
        {
            _toggle.onValueChanged.AddListener(UpdateUI);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(UpdateUI);
        }

        private void UpdateUI(bool isOn)
        {            
            if (isOn)
            {
                _panel.gameObject.SetActive(true);
            }

            _panel.DOFade(isOn ? 1 : 0, _fadeDuration).OnComplete(() =>
            {
                _panel.gameObject.SetActive(isOn);
            });

            _scrollViewAnimator.Toggle(!isOn, _fadeDuration);
        }
    }
}

