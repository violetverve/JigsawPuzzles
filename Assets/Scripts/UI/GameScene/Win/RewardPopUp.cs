using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;
using DG.Tweening;
using Player;
using UI;
using JigsawPuzzles.Services.Ads;
using UnityEngine.UI;

namespace UI.GameScene.Win
{
    public class RewardPopUp : MonoBehaviour
    {
        [SerializeField] private Transform _rewardPopUpWindow;
        [SerializeField] private CoinsCollectAnimator _coinsCollectAnimator;
        [SerializeField] private RewardTag _rewardTag;
        [SerializeField] private int _rewardMultiplier = 3;

        [SerializeField] private Button _rewardedClaimButton;
        [SerializeField] private Button _claimButton;

        private int _reward;

        private void OnEnable()
        {
           AdsManager.Instance.RewardedAdWatched += HandleRewardedAdWatched;
        }

        private void OnDisable()
        {
            AdsManager.Instance.RewardedAdWatched -= HandleRewardedAdWatched;
        }

        private void Start()
        {
            AnimateRewardPopUp();
        }

        private void AnimateRewardPopUp()
        {
            _rewardPopUpWindow.localScale = Vector3.zero;
            _rewardPopUpWindow.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        public void SetReward(int reward)
        {
            _rewardTag.SetRewardText(reward);
            _reward = reward;
        }

        public void ClaimPrize()
        {
            if (PlayerData.Instance != null)
            {
                Debug.Log("Updating coins from ClaimPrize");
                PlayerData.Instance.UpdateCoins(_reward);
            }
            else
            {
              _reward = 100;
            }

            var sequence = _coinsCollectAnimator.GetCoinsCollectSequence(_reward)
                                                .OnComplete(LoadUIScene);

            sequence.Play();
        }

        private void LoadUIScene()
        {
            SceneManager.LoadScene("UIScene");
        }

        public void Claim()
        {
            SetClaimButtonsInteractible(false);

            ClaimPrize();
        }

        public void ClaimRewarded()
        {
            if (AdsManager.Instance.IsRewardedAdAvailable())
            {
                AdsManager.Instance.ShowRewarded();

                SetClaimButtonsInteractible(false);
            }
            else
            {
                HandleRewardedAdWatched();
            }
        }

        private void HandleRewardedAdWatched()
        {
            Sequence rewardSequence = DOTween.Sequence();

            rewardSequence
                .Append(_rewardTag.GetPumpSequence(_rewardMultiplier, _reward))
                .AppendCallback(MultiplyReward)
                .AppendCallback(ClaimPrize);

            rewardSequence.Play();
        }

        private void MultiplyReward()
        {
            _reward *= _rewardMultiplier;
        }

        private void SetClaimButtonsInteractible(bool interactible)
        {
            _claimButton.interactable = interactible;
            _rewardedClaimButton.interactable = interactible;
        }

    }
}