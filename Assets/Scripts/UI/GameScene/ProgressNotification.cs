using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.GameScene
{
    public class ProgressNotification : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _progressNotification;
        [SerializeField] private float _fadeDuration = 0.5f;

        private Tween Show()
        {
            gameObject.SetActive(true);
            return _progressNotification.DOFade(1, _fadeDuration);
        }

        private Tween Hide()
        {
            gameObject.SetActive(false);
            return _progressNotification.DOFade(0, _fadeDuration);
        }

        public void Animate()
        {
            Show().OnComplete(() => 
            {
                StartCoroutine(HideAfterDelay());
            });
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(2);
            Hide();
        }
    }
}
