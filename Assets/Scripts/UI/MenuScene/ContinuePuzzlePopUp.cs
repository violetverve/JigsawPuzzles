using UnityEngine;
using UI.MenuScene.Puzzle;
using JigsawPuzzles.PuzzleData;
using JigsawPuzzles.UI.MenuScene;
using JigsawPuzzles.Services.Firebase.Firestore;

namespace UI.MenuScene
{
    public class ContinuePuzzlePopUp : MonoBehaviour
    {
        [SerializeField] private PuzzlePanelUI _puzzleImagePanel;
        [SerializeField] private LoadPuzzleButton _loadPuzzleButton;
        private PuzzleDocument _puzzle;

        public void ActivatePopUp(PuzzleDocument puzzle)
        {
            _puzzle = puzzle;
            _puzzleImagePanel.LoadPuzzlePanel(puzzle);
            gameObject.SetActive(true);

            SetupPuzzleSprite(puzzle);
        }

        public void LoadGameScene()
        {
            UIManager.OnPuzzleContinue?.Invoke(_puzzle);
            gameObject.SetActive(false);
        }

        public void RestartPuzzle()
        {
            LoadDifficultyPanel();
        }

        public void LoadDifficultyPanel()
        {     
            UIManager.OnPanelClick?.Invoke(_puzzle);
            gameObject.SetActive(false);
        }

        private async void SetupPuzzleSprite(PuzzleDocument puzzle)
        {
            if (puzzle.FullSizeSprite == null)
            {
                _loadPuzzleButton.StartLoading();

                var sprite = await FirestoreManager.DownloadImageAndGetSprite(puzzle.FullSizeImagePath);
                puzzle.SetFullSizeSprite(sprite);

                _loadPuzzleButton.StopLoading();
            }
        }

    }
}