using UnityEngine;
using UnityEngine.UI;
using GameManagement;
using DG.Tweening;

namespace UI.GameScene
{
    public class ProgressBar : MonoBehaviour
    {

        [SerializeField] private Slider _slider;
        [SerializeField] private float _minShowProgress;
        private float _slideDuration = 0.5f;

        private void OnEnable()
        {
            ProgressManager.ProgressUpdated += HandleProgressUpdate;
            ProgressManager.ProgressLoaded += HandleProgressLoad;
        }

        private void OnDisable()
        {
            ProgressManager.ProgressUpdated -= HandleProgressUpdate;
            ProgressManager.ProgressLoaded -= HandleProgressUpdate;
        }

        private void HandleProgressLoad(float progress)
        {
            _slider.value = GetShowProgressValue(progress);

        }

        private void HandleProgressUpdate(float progress)
        {
            _slider.DOValue(GetShowProgressValue(progress), _slideDuration);
        }

        private float GetShowProgressValue(float progress)
        {
            if (progress == 0)
            {
                return 0;
            }

            return Mathf.Max(progress, _minShowProgress);
        }   
    }

}
