using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagement;
using DG.Tweening;
using UnityEngine.UI;
using GameManagement.Difficulty;

namespace UI.GameScene.Win
{
    public class WinAnimator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _objectsToDisable;
        [SerializeField] private GameObject _puzzleGrid;
        [SerializeField] private Image _gridFrame;
        [SerializeField] private RectTransform _gridFrameRectTransform;
        [SerializeField] private Image _puzzleImage;
        [SerializeField] private CanvasGroup _backgroundGroup;
        [SerializeField] private CanvasGroup _frontGroup;
        [SerializeField] private Button _nextButton;
        [SerializeField] private ParticleSystem _confetti;
        [SerializeField] private List<ParticleSystem> _shines;
        [SerializeField] private RewardPopUp _rewardPopUp;
        [SerializeField] private DifficultyManagerSO _difficultyManager;
        [SerializeField] private float _animationDuration = 0.8f;
        [SerializeField] private float _lightShineDuration = 0.3f;
        [SerializeField] private LevelManager _levelManager;
        private int _reward;

        private void OnEnable()
        {
            ProgressManager.Win += HandleWin;
        }

        private void OnDisable()
        {
            ProgressManager.Win -= HandleWin;
        }

        private void Start()
        {
            var currentLevel = _levelManager.CurrentLevel;
            _reward = _difficultyManager.GetRewardByGridSO(currentLevel.GridSO);
            _puzzleImage.sprite = currentLevel.PuzzleData.FullSizeSprite;
        }

        private void HandleWin()
        {
            foreach (var obj in _objectsToDisable)
            {
                obj.SetActive(false);
            }

            _nextButton.interactable = true;

            _backgroundGroup.gameObject.SetActive(true);

            StartCoroutine(AnimateGrid());
        }
    
        private IEnumerator AnimateGrid()
        {
            yield return new WaitForSeconds(0.2f);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_puzzleGrid.transform.DOScale(0.9f, _animationDuration))
                .AppendCallback(SetupGrid)
                .Append(_gridFrame.DOFade(1, _animationDuration))
                .AppendCallback(AnimateGridAndFrame)
                .Append(_backgroundGroup.DOFade(1, _animationDuration))
                .Append(_frontGroup.DOFade(1, _lightShineDuration))
                .AppendCallback(PlayEffects);
        }

        private void AnimateGridAndFrame()
        {
            _puzzleGrid.transform.DOScale(0.65f, _animationDuration);
            _gridFrame.gameObject.transform.DOScale(0.72f, _animationDuration);
            _gridFrameRectTransform.DOAnchorPosY(80, _animationDuration);
        }

        private void PlayEffects()
        {
            _confetti.Play();
            _shines.ForEach(shine => shine.Play());
        }

        private void SetupGrid()
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            SetLayerIteratively(_puzzleGrid, uiLayer);
            
            Vector3 newPosition = _puzzleGrid.transform.position;
            newPosition.z = 1;
            _puzzleGrid.transform.position = newPosition;
        }

        private void SetLayerIteratively(GameObject root, int newLayer)
        {
            if (root == null) return;

            var nodes = new Queue<Transform>();
            nodes.Enqueue(root.transform);

            while (nodes.Count > 0)
            {
                Transform current = nodes.Dequeue();
                current.gameObject.layer = newLayer;

                foreach (Transform child in current)
                {
                    nodes.Enqueue(child);
                }
            }
        }

        public void ShowRewardPopUp()
        {
            _puzzleImage.gameObject.SetActive(true);
            _puzzleGrid.SetActive(false);
            _rewardPopUp.gameObject.SetActive(true);
            _rewardPopUp.SetReward(_reward);
        }

    }
}