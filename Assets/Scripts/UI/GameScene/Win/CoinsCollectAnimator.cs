using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


namespace UI.GameScene.Win
{
    public class CoinsCollectAnimator : MonoBehaviour
    {
        [SerializeField] private Coins _coinsTarget;
        [SerializeField] private Transform _coinsSource;
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private RewardTag _rewardTag;
        [SerializeField] private int _coinsToAnimate = 10;
        [SerializeField] private float _pumpCoinScale = 1.2f;
        [SerializeField] private float _pumpDuration = 0.2f;
        [SerializeField] private int _pumps = 3;

        private Sequence GetPumpSequence()
        {
            Sequence pumpSequence = DOTween.Sequence();
            pumpSequence.Append(_coinsSource.DOScale(Vector3.one * _pumpCoinScale, _pumpDuration)
                    .SetEase(Ease.OutBounce))
                .Append(_coinsSource.DOScale(Vector3.one, _pumpDuration)
                    .SetEase(Ease.InBounce))
                .SetLoops(_pumps, LoopType.Restart);

            return pumpSequence;
        }

        public Sequence GetCoinsCollectSequence(int reward)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(GetPumpSequence());
        
            int coinsPerAnimation = reward / _coinsToAnimate; 
            
            for (int i = 0; i < _coinsToAnimate; i++)
            {
                var coin = Instantiate(_coinPrefab, _coinsSource.position, Quaternion.identity, transform);
                int currentIndex = i;

                var moveTween = coin.transform.DOMove(_coinsTarget.CoinsImage.transform.position, 1f)
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(0.1f)
                    .OnComplete(() => 
                    {
                        Destroy(coin);
                        _coinsTarget.AddCoinsText(coinsPerAnimation);
            
                        if (currentIndex == _coinsToAnimate - 2)
                        {
                            sequence.Append(_rewardTag.GetFadeOutTween(0.2f));
                        }
                        else
                        {
                            _rewardTag.SetRewardText(reward - coinsPerAnimation * (currentIndex + 1));
                        }
                    });
            
                sequence.Join(moveTween);
            }

            return sequence;
        }
    }
}