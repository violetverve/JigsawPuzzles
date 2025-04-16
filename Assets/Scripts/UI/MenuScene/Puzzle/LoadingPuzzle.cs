using DG.Tweening;
using UnityEngine;

namespace UI.MenuScene.Puzzle
{
    public class LoadingPuzzle : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration;

        private void OnEnable()
        {
            _canvasGroup.alpha = 1;
        }

        public void HideLoading()
        {
            _canvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

    }
}
