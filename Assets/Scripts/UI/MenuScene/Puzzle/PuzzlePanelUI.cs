using UnityEngine;
using UnityEngine.UI;
using Player;
using JigsawPuzzles.PuzzleData;

namespace UI.MenuScene.Puzzle
{
    public class PuzzlePanelUI : MonoBehaviour
    {
        [SerializeField] private Image _puzzleUIImage;
        [SerializeField] private GameObject _lockImage;
        [SerializeField] private GameObject _completedImage;
        [SerializeField] private ProgressTag _progressTag;
        [SerializeField] private SecretLevel _secretLevelUI;
        [SerializeField] private LoadingPuzzle _loadingPanel;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _button;

        private int _puzzleID;
        private bool _locked;
        private bool _inProgress;
        private float _progressPercentage;
        private PuzzleDocument _puzzleData;

        public int PuzzleID => _puzzleID;

        public async void LoadPuzzlePanel(PuzzleDocument puzzleData)
        {
            if (puzzleData is PuzzleDocument puzzleDocument)
            {
                if (puzzleDocument.PreviewSprite == null)
                {
                    _loadingPanel.gameObject.SetActive(true);
                    await puzzleDocument.LoadPreviewSprite();
                    _loadingPanel.HideLoading();
                }
            }

            Initialize(puzzleData);
        }

        public void Reload()
        {
            Initialize(_puzzleData);
        }

        private void Initialize(PuzzleDocument puzzleDocument)
        {
            RestoreFromPlaceholder();

            bool previouslyUsed = _puzzleID != 0;

            if (previouslyUsed)
            {
                _progressTag?.gameObject.SetActive(false);
                _lockImage?.SetActive(false);
                _completedImage?.SetActive(false);
            }

            _puzzleID = puzzleDocument.Id;
            _locked = puzzleDocument.IsLocked;
            _inProgress = false;
            _progressPercentage = 0;
            _puzzleData = puzzleDocument;

            LoadPuzzleImage(puzzleDocument);
            LoadSecretLevel(puzzleDocument);
            UpdateLockStatus(puzzleDocument);
            LoadProgress(puzzleDocument);

            _lockImage.SetActive(_locked);
        }

        private void LoadSecretLevel(PuzzleDocument puzzleDocument)
        {
            _secretLevelUI.LoadSecretLevel(puzzleDocument.IsSecret, puzzleDocument.Id);
        }

        private void LoadPuzzleImage(PuzzleDocument puzzleDocument)
        {
            if (!puzzleDocument.IsSecret)
            {
                _puzzleUIImage.sprite = puzzleDocument.PreviewSprite;
                _puzzleUIImage.color = Color.white;
            }
        }

        private void UpdateLockStatus(PuzzleDocument puzzleDocument)
        {
            if (puzzleDocument.IsLocked && PlayerData.Instance.IsPuzzleUnlocked(puzzleDocument.Id))
            {
                _locked = false;
            }

            if (PlayerData.Instance.TryGetSavedPuzzle(puzzleDocument.Id) != null)
            {
                _locked = false;
            }
        }

        private void LoadProgress(PuzzleDocument puzzleDocument)
        {
            var progress = PlayerData.Instance.GetPuzzleProgress(puzzleDocument.Id);
            if (progress != -1)
            {
                _inProgress = true;
                _progressPercentage = progress;

                if (_progressTag != null)
                {
                    if (_progressPercentage == 100)
                    {
                        _completedImage.SetActive(true);
                    }
                    else
                    {
                        _progressTag.gameObject.SetActive(true);
                        _progressTag.SetProgressText(progress);
                    }
                }
            }
        }

        public void LoadPuzzlePopUp()
        {
            if (_locked)
            {
                PopUpManager.OnLockedPanelClick?.Invoke(_puzzleData);
            }
            else if (_inProgress && _progressPercentage < 100)
            {
                PopUpManager.OnContinuePanelClick?.Invoke(_puzzleData);
            }
            else
            {
                UIManager.OnPanelClick?.Invoke(_puzzleData);
            }
        }

        public void SetLocked(bool locked)
        {
            _locked = locked;
            _lockImage.SetActive(_locked);
        }

        public void SetPlaceholderState()
        {
            _button.interactable = false;
            _canvasGroup.alpha = 0;
        }

        private void RestoreFromPlaceholder()
        {
            _button.interactable = true;
            _canvasGroup.alpha = 1;
        }
    }

}