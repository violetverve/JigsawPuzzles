using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace JigsawPuzzles.UI.LoadScene
{
    public class LogoAnimator : MonoBehaviour
    {
        [SerializeField] private TextAnimator _textAnimator;
        [SerializeField] private PopAnimation _popAnimation;

        [SerializeField] private float _waitForSeconds = 1f;
        [SerializeField] private float _popDuration = 1.2f;
        [SerializeField] private float _textAnimationDelay = 0.6f;

        public static event Action LogoAnimationCompleted;

        void Start()
        {
            AnimateLogo();
        }

        public void AnimateLogo()
        {
            StartCoroutine(RunAnimation(_waitForSeconds));
        }

        private IEnumerator RunAnimation(float waitForSeconds)
        {
            yield return new WaitForSeconds(waitForSeconds);

            _popAnimation.gameObject.SetActive(true);
            Tween popTween = _popAnimation.GetPopTween(_popDuration);
            popTween.Play();

            StartCoroutine(_textAnimator.RunAnimation(_textAnimationDelay));

            yield return popTween.WaitForCompletion();

            var waitDuration = _textAnimator.AnimationDuration + _textAnimationDelay - _popDuration;

            yield return new WaitForSeconds(waitDuration);

            LogoAnimationCompleted?.Invoke();
        }

    }

}