using UnityEngine;
using UnityEngine.UI;
using Player;
using UI.MenuScene.Puzzle;
using JigsawPuzzles.PuzzleData;
using System;
using TMPro;

namespace UI.MenuScene
{
    public class UnlockPuzzlePopUp : MonoBehaviour
    {
        private const string SecretPuzzleText = "Get Secret Puzzle";
        private const string NormalPuzzleText = "Get This Puzzle";

        [SerializeField] private PuzzlePanelUI _puzzleImagePanel;
        [SerializeField] private int _unlockPrice = 1000;
        [SerializeField] private int _unlockSecretPrice = 2000;
        [SerializeField] private Button _unlockButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _unlockPriceText;

        private int _currentUnlockPrice;

        private PuzzleDocument _puzzleData;

        public static Action<int> PuzzleUnlocked;

        public void ActivatePopUp(PuzzleDocument puzzle)
        {
            _puzzleData = puzzle;
            _puzzleImagePanel.LoadPuzzlePanel(puzzle);

            SetupCurrentUnlockPrice();
            SetupCurrentTitleText();
            SetupUnlockButton();
            gameObject.SetActive(true);
        }

        public void TryUnlockPuzzle()
        {
            if (PlayerData.Instance.TryUnlockPuzzle(_currentUnlockPrice, _puzzleData.Id))
            {
                PuzzleUnlocked?.Invoke(_puzzleData.Id);
                Coins.CoinsChanged?.Invoke();
                LoadDifficultyPanel();
            }
        }

        public void LoadDifficultyPanel()
        {
            UIManager.OnPanelClick?.Invoke(_puzzleData);
            gameObject.SetActive(false);
        }

        private void SetupUnlockButton()
        {
            var interactable = PlayerData.Instance.Coins >= _currentUnlockPrice;

            _unlockButton.interactable = interactable;
        }

        private void SetupCurrentUnlockPrice()
        {
            _currentUnlockPrice = _puzzleData.IsSecret ? _unlockSecretPrice : _unlockPrice;
            _unlockPriceText.text = _currentUnlockPrice.ToString();
        }

        private void SetupCurrentTitleText()
        {
            _titleText.text = _puzzleData.IsSecret ? SecretPuzzleText : NormalPuzzleText;
        }
    }
}
