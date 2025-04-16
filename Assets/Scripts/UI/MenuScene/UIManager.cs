using System.Collections.Generic;
using UnityEngine;
using Player;
using System;
using GameManagement;
using UnityEngine.SceneManagement;
using GameManagement.Difficulty;
using System.Linq; 
using JigsawPuzzles.PuzzleData;


namespace UI.MenuScene
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PuzzlePrepareUI _puzzlePrepareUI;
        [SerializeField] private DifficultyManagerSO _difficultyManager;

        [Header("Scroll")]
        [SerializeField] private GameObject _scrollParent;
        [SerializeField] private ScrollElement _scrollPrefab;

        [Header("Scroll Managers")]
        [SerializeField] private ScrollManager _homeScrollManager;
        [SerializeField] private ScrollManager _playerPuzzlesScrollManager;

        private List<PuzzleDocument> _puzzlesData = new List<PuzzleDocument>();
        private PuzzleDocument _currentPuzzleData;
        private DifficultySO _currentDifficulty;
        private bool _currentRotationEnabled;

        public static Action<GameObject> OnCrossClick;
        public static Action<PuzzleDocument> OnPanelClick;
        public static Action<PuzzleDocument> OnPuzzleContinue;

        public List<PuzzleDocument> PuzzlesData => _puzzlesData;


        private void OnEnable()
        {
            OnCrossClick += CloseWindow;
            OnPanelClick += LoadPuzzleDifficultyChooser;
            PuzzlePrepareUI.ScrollActiveItemChanged += SetCurrentDifficulty;
            UnlockPuzzlePopUp.PuzzleUnlocked += UnlockPuzzleUIPanel;
            OnPuzzleContinue += LoadPuzzle;
        }

        private void OnDisable()
        {
            OnCrossClick -= CloseWindow;
            OnPanelClick -= LoadPuzzleDifficultyChooser;
            PuzzlePrepareUI.ScrollActiveItemChanged -= SetCurrentDifficulty;
            UnlockPuzzlePopUp.PuzzleUnlocked -= UnlockPuzzleUIPanel;
            OnPuzzleContinue -= LoadPuzzle;
        }

        private void Start()
        {
            LoadHomeScroll();
            LoadDifficulties();
            SetCurrentDifficulty(0);
        }

        private void LoadHomeScroll()
        {
            var allDocuments = DocumentLoader.Instance.PuzzleDocuments;

            var notCompletedPuzzles = allDocuments
                    .Where(p => !PlayerData.Instance.IsPuzzleCompleted(p.Id))
                    .ToList();

            _homeScrollManager.Initialize(notCompletedPuzzles);
        }

        private void UnlockPuzzleUIPanel(int id)
        {
            _homeScrollManager.UpdatePuzzlePanel(id);
        }
 
        public void LoadPuzzle(PuzzleDocument puzzleData)
        {
            Debug.Log("LoadPuzzle");
            var puzzleSave = PlayerData.Instance.TryGetSavedPuzzle(puzzleData.Id);
            var gridSO = _difficultyManager.GetGridSOBySide(puzzleSave.GridSide);
            var rotationEnabled = puzzleSave.RotationEnabled;

            if (puzzleSave != null)
            {
                Debug.Log("PuzzleSave found loading");
                Level level = new Level(gridSO, puzzleData, rotationEnabled);
                PlayerData.Instance.SetCurrentLevel(level);
                SceneManager.LoadScene("Main");
            }
        }

        public void StartPuzzle()
        {
            PlayerData.Instance.DeleteSavedPuzzle(_currentPuzzleData.Id);

            Level level = new Level(_currentDifficulty.Grid, _currentPuzzleData, _currentRotationEnabled);
            PlayerData.Instance.SetCurrentLevel(level);
            SceneManager.LoadScene("Main");
        }

        public void ToggleRotationEnabled()
        {
            _currentRotationEnabled = !_currentRotationEnabled;
        }

        private void LoadDifficulties()
        {
            int difficultiesNumber = _difficultyManager.Difficulties.Count;

            for (int i = 0; i < difficultiesNumber; i++)
            {
                var difficulty = Instantiate(_scrollPrefab, _scrollParent.transform);
                difficulty.Load(_difficultyManager.Difficulties[i], i);
            }
        }

        #region MenuButtonsInteraction

        private void CloseWindow(GameObject ToClose)
        {
            ToClose.SetActive(false);
        }

        #endregion

        #region ChoosePuzzleInteraction
        public void LoadPuzzleDifficultyChooser(PuzzleDocument puzzleData)
        {
            var puzzleDocument = DocumentLoader.Instance.PuzzleDocuments.Find(p => p.Id == puzzleData.Id);
            _puzzlePrepareUI.Initialize(puzzleDocument);
            _puzzlePrepareUI.gameObject.SetActive(true);
            _currentPuzzleData = puzzleData;
        }

        private void SetCurrentDifficulty(int index)
        {
            _currentDifficulty = _difficultyManager.GetDifficulty(index);
        }

        #endregion

        public void OnPlayerPuzzlesTabOpened()
        {
            if (!_playerPuzzlesScrollManager.IsInitialized())
            {
                var startedPuzzleIds = new HashSet<int>(PlayerData.Instance.SavedPuzzles.Select(p => p.Id));

                var orderedStartedPuzzles = GetOrderedPuzzles(startedPuzzleIds.ToList());
                var orderedCompletedPuzzles = GetOrderedPuzzles(PlayerData.Instance.CompletedPuzzles);

                orderedStartedPuzzles.AddRange(orderedCompletedPuzzles);

                _playerPuzzlesScrollManager.Initialize(orderedStartedPuzzles);
            }
        }


        private List<PuzzleDocument> GetOrderedPuzzles(List<int> puzzlesIds)
        {
            return puzzlesIds
                .Select(puzzleId => DocumentLoader.Instance.PuzzleDocuments
                    .FirstOrDefault(doc => doc.Id == puzzleId))
                .Where(doc => doc != null)
                .Reverse()
                .ToList();
        }

    }
}

