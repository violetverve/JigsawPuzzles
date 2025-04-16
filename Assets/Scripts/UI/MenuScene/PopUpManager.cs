using UnityEngine;
using System;
using JigsawPuzzles.PuzzleData;

namespace UI.MenuScene
{
    public class PopUpManager : MonoBehaviour
    {
        [SerializeField] private UnlockPuzzlePopUp _unlockPuzzlePopUp;
        [SerializeField] private ContinuePuzzlePopUp _continuePuzzlePopUp;

        public static Action<PuzzleDocument> OnLockedPanelClick;
        public static Action<PuzzleDocument> OnContinuePanelClick;

        private void OnEnable()
        {
            OnLockedPanelClick += LoadUnlockPanel;
            OnContinuePanelClick += LoadContinuePanel;
        }

        private void OnDisable()
        {
            OnLockedPanelClick -= LoadUnlockPanel;
            OnContinuePanelClick -= LoadContinuePanel;
        }

        public void LoadUnlockPanel(PuzzleDocument puzzleData)
        {
            _unlockPuzzlePopUp.ActivatePopUp(puzzleData);
        }

        public void LoadContinuePanel(PuzzleDocument puzzleData)
        {
            _continuePuzzlePopUp.ActivatePopUp(puzzleData);
        }

    }
}
