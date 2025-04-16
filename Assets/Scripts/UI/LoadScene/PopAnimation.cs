using DG.Tweening;
using UnityEngine;

namespace JigsawPuzzles.UI.LoadScene
{
    public class PopAnimation : MonoBehaviour
    {
        public Tween GetPopTween(float duration)
        {
            var startScale = 0.3f;
            var _easeType = Ease.OutQuad;
            transform.localScale = Vector3.one * startScale;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOScale(1.2f, duration * 0.25f).SetEase(_easeType));
            sequence.Append(transform.DOScale(0.8f, duration * 0.2f).SetEase(_easeType));
            sequence.Append(transform.DOScale(1.05f, duration * 0.2f).SetEase(_easeType));
            sequence.Append(transform.DOScale(0.95f, duration * 0.15f).SetEase(_easeType));
            sequence.Append(transform.DOScale(1f, duration * 0.2f).SetEase(_easeType));

            return sequence;
        }

    }
}
