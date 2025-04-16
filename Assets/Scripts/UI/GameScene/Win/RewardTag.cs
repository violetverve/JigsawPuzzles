using UnityEngine;
using TMPro;
using DG.Tweening;

namespace UI.GameScene.Win
{
    public class RewardTag : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private CanvasGroup _rewardCanvasGroup;
        [SerializeField] private float _rewardTagScale = 1.2f;
        [SerializeField] private float _pumpDuration = 0.2f;
        private int _reward;
        private int _pumpReward;

        public void SetRewardText(int reward)
        {
            _reward = reward;
            _rewardText.text = "+ " + reward.ToString();
        }

        public void FadeOut(float duration)
        {
            _rewardCanvasGroup.DOFade(0, duration).OnComplete(() => gameObject.SetActive(false));
        }

        public Tween GetFadeOutTween(float duration)
        {
            return _rewardCanvasGroup.DOFade(0, duration);
        }

        public Sequence GetPumpSequence(int multiplier, int originalReward)
        {
            Sequence pumpSequence = DOTween.Sequence();

            int numberOfPumps = multiplier - 1;
            _pumpReward = originalReward;

            pumpSequence.Append(transform.DOScale(Vector3.one * _rewardTagScale, _pumpDuration)
                    .SetEase(Ease.OutBounce))
                .Append(transform.DOScale(Vector3.one, _pumpDuration).SetEase(Ease.InBounce))
                .OnStepComplete(OnPumpStepComplete)
                .SetLoops(numberOfPumps, LoopType.Restart);

            return pumpSequence;
        }

        private void OnPumpStepComplete()
        {
            AddToRewardText(_pumpReward);
        }

        private void AddToRewardText(int value)
        {
            _reward += value;
            _rewardText.text = "+ " + _reward.ToString();
        }
    }
}