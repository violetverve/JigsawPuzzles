using UnityEngine;
using System;

namespace JigsawPuzzles.Services.Ads
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance;
        public event Action RewardedAdWatched;
        private string _appId;
        private const string IronSourceMediationSettings = "IronSourceMediationSettings";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            IronSourceMediationSettings settings = Resources.Load<IronSourceMediationSettings>(IronSourceMediationSettings);

            if (settings != null ) {
            #if UNITY_ANDROID
                        _appId = settings.AndroidAppKey;
            #elif UNITY_IOS
                        _appId = settings.IOSAppKey;
            #endif
            
            }

            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(_appId);
        }


        private void OnEnable()
        {
            SubscribeRewardedCallbacks();
        }

        private void OnDisable()
        {
            UnsubscribeRewardedCallbacks();
        }


        private void OnApplicationPause(bool pause)
        {
            IronSource.Agent.onApplicationPause(pause);
        }

        #region Rewarded

        private void SubscribeRewardedCallbacks()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        }

        private void UnsubscribeRewardedCallbacks()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        }

        public void LoadRewarded()
        {
            IronSource.Agent.loadRewardedVideo();
        }

        public void ShowRewarded()
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("Rewarded Video is not available");
            }
        }

        public bool IsRewardedAdAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            RewardedAdWatched?.Invoke();
        }

        #endregion
    }

}
