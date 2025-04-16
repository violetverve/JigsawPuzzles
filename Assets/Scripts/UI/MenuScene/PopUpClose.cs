using UnityEngine;
using DG.Tweening;

namespace UI.MenuScene
{
    public class PopUpClose : MonoBehaviour
    {
        [SerializeField] GameObject _popUpPanel;
        [SerializeField] GameObject _popUpWindow;
        [SerializeField] float _duration = 0.25f;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = _popUpPanel.GetComponent<CanvasGroup>();
        }

        public void Close()
        {
            AnimateClose();
        }

        private void AnimateClose()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_popUpWindow.transform.DOScale(0, _duration).SetEase(Ease.InBack))
                    .Join(_canvasGroup.DOFade(0, _duration))
                    .OnComplete(DisablePopUp);
        }

        private void DisablePopUp()
        {
            _popUpPanel.SetActive(false);
            _popUpWindow.transform.localScale = Vector3.one;
            _canvasGroup.alpha = 1;
        }
    }
}

