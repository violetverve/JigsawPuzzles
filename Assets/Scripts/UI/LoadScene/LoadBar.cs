using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using JigsawPuzzles.PuzzleData;

namespace JigsawPuzzles.UI.LoadScene
{
    public class LoadBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private float _loadTime = 2;
        [SerializeField] DocumentLoader _documentLoader;

        public static Action LoadCompleted;

        private void OnEnable()
        {
            _documentLoader.LoadProgress += HandleLoadProgress;
        }

        private void OnDisable()
        {
            _documentLoader.LoadProgress -= HandleLoadProgress;
        }
        private void HandleLoadProgress(float progress)
        {
            DOTween.To(GetSliderValue, SetSliderValue, progress, _loadTime * progress)
                .SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if (progress == 1)
                    {
                        LoadCompleted?.Invoke();
                    }
                });
        }

        private float GetSliderValue()
        {
            return _slider.value;
        }

        private void SetSliderValue(float value)
        {
            _slider.value = value;
        }
    }
}
