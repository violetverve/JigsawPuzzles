using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using GameManagement;
using PuzzleData.Save;
using System.Linq;

namespace Player
{
    public class PlayerData : MonoBehaviour
    {

        private readonly string _coinsPrefs = "player_coins";
        private readonly string _hintsPrefs = "player_hints";
        private readonly string _savedPuzzlesPref = "player_savedPuzzle";
        private readonly string _themePref = "player_theme";
        private readonly string _unlockedPuzzlesPref = "unlocked_puzzles";
        private readonly string _completedPuzzlesPref= "completed_puzzles";

        private int _coins;
        private int _hints;

        private List<PuzzleSave> _savedPuzzles;

        private int _themeID;
        private Level _currentLevel;

        private List<int> _unlockedPuzzles;
        private List<int> _completedPuzzles;

        public static PlayerData Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllPlayerData();
        }

        public void SetCurrentLevel(Level level)
        {
            _currentLevel = level;
        }

        #region Saving

        public void LoadAllPlayerData()
        {
            _coins = PlayerPrefs.GetInt(_coinsPrefs, 5000);
            _hints = PlayerPrefs.GetInt(_hintsPrefs, 30);
            _themeID = PlayerPrefs.GetInt(_themePref, 0);

            LoadSavedPuzzled();
            LoadUnlockedPuzzles();
            LoadCompletedPuzzles();

            Debug.Log("Player data loaded");
        }

        private void LoadSavedPuzzled()
        {
            string savedPuzzles = PlayerPrefs.GetString(_savedPuzzlesPref);


            if (string.IsNullOrEmpty(savedPuzzles))
            {
                _savedPuzzles = new List<PuzzleSave>();
            }
            else
            {
                _savedPuzzles = JsonConvert.DeserializeObject<List<PuzzleSave>>(savedPuzzles);
            }
        }

        private void LoadUnlockedPuzzles()
        {
            var unlockedPuzzles = PlayerPrefs.GetString(_unlockedPuzzlesPref);
            if (string.IsNullOrEmpty(unlockedPuzzles))
            {
                _unlockedPuzzles = new List<int>();
            }
            else
            {
                _unlockedPuzzles = JsonConvert.DeserializeObject<List<int>>(unlockedPuzzles);
            }
        }

        private void LoadCompletedPuzzles()
        {
            var completedPuzzles = PlayerPrefs.GetString(_completedPuzzlesPref);
            if (string.IsNullOrEmpty(completedPuzzles))
            {
                _completedPuzzles = new List<int>();
            }
            else
            {
                _completedPuzzles = JsonConvert.DeserializeObject<List<int>>(completedPuzzles);
            }
        }

        public void SetCurrentLevelSolved()
        {
            int puzzleId = _currentLevel.PuzzleData.Id;
            if (!_completedPuzzles.Contains(puzzleId))
            {
                DeleteSavedPuzzle(puzzleId);

                _completedPuzzles.Add(puzzleId);
                SaveCompletedPuzzles();
            }
        }

        public void SaveUnlockedPuzzles()
        {
            PlayerPrefs.SetString(_unlockedPuzzlesPref, JsonConvert.SerializeObject(_unlockedPuzzles));
            PlayerPrefs.Save();
        }

        public void SaveCompletedPuzzles()
        {
            PlayerPrefs.SetString(_completedPuzzlesPref, JsonConvert.SerializeObject(_completedPuzzles));
            PlayerPrefs.Save();
        }

        public bool IsPuzzleUnlocked(int id)
        {
            return _unlockedPuzzles.Contains(id);
        }

        public void SaveThemeID(int id)
        {
            _themeID = id;
            PlayerPrefs.SetInt(_themePref, id);
            PlayerPrefs.Save();
        }

        public void AddSavedPuzzle(PuzzleSave puzzle)
        {
            var previouslySavedPuzzle = TryGetSavedPuzzle(puzzle.Id);
           
            if (previouslySavedPuzzle != null)
            {
                _savedPuzzles.Remove(previouslySavedPuzzle);
            }

            _savedPuzzles.Add(puzzle);
            SaveSavedPuzzles();
        }

        public void DeleteSavedPuzzle(int id)
        {
            var savedPuzzle = _savedPuzzles.Find(p => p.Id == id);

            if (savedPuzzle != null)
            {
                Debug.Log("Deleting saved puzzle");
                _savedPuzzles.Remove(savedPuzzle);
                SaveSavedPuzzles();
            }
        }

        public void DeleteCompletedPuzzle(int id)
        {
            _completedPuzzles.Remove(id);

            SaveCompletedPuzzles();
        }

        public void SaveSavedPuzzles()
        {
            if (_savedPuzzles != null)
            {
                string savedPuzzles = JsonConvert.SerializeObject(_savedPuzzles);
                PlayerPrefs.SetString(_savedPuzzlesPref, savedPuzzles);
                PlayerPrefs.Save();
            }
        }

        public PuzzleSave TryGetSavedPuzzle(int id)
        {
            if (_savedPuzzles == null)
            {
                return null;
            }

            var ids = _savedPuzzles.Select(p => p.Id).ToList();

            if (ids.Contains(id))
            {
                return _savedPuzzles[ids.IndexOf(id)];
            }

            return null;
        }

        public int GetPuzzleProgress(int id)
        {
            if (IsPuzzleCompleted(id))
            {
                return 100;
            }

            var savedPuzzle = TryGetSavedPuzzle(id);
            if (savedPuzzle == null)
            {
                return -1;
            }

            int collectedPiecesCount = savedPuzzle.CollectedPieceSaves.Count;
            int gridSide = savedPuzzle.GridSide;
            int totalPieces = gridSide * gridSide;
            int percentageCollected = (int)(((float)collectedPiecesCount / totalPieces) * 100);

            return percentageCollected;
        }

        public bool IsPuzzleCompleted(int id)
        {
            return _completedPuzzles.Contains(id);
        }

        #endregion

        #region UpdateConsumables
        public void AddCoins(int reward)
        {
            _coins += reward;
            PlayerPrefs.SetInt(_coinsPrefs, _coins);
            PlayerPrefs.Save();
        }

        public void UpdateCoins(int amount)
        {
            _coins += amount;
            PlayerPrefs.SetInt(_coinsPrefs, _coins);
            PlayerPrefs.Save();

            Debug.Log("Coins Updated: " + _coins);
        }

        public void UseHint()
        {
            _hints--;
            PlayerPrefs.SetInt(_hintsPrefs, _hints);
            PlayerPrefs.Save();
        }

        # endregion

        #region UnlockPuzzle
        public bool TryUnlockPuzzle(int price, int id)
        {
            if (_coins >= price)
            {
                UpdateCoins(-price);
                UnlockPuzzle(id);
                return true;
            }

            return false;
        }

        public void UnlockPuzzle(int id)
        {
            if (!_unlockedPuzzles.Contains(id))
            {
                _unlockedPuzzles.Add(id);
                SaveUnlockedPuzzles();
            }
        }

        #endregion


        public int Coins => _coins;
        public int Hints => _hints;

        public List<PuzzleSave> SavedPuzzles => _savedPuzzles;
        public int ThemeID => _themeID;
        public Level CurrentLevel => _currentLevel;
        public List<int> UnlockedPuzzles => _unlockedPuzzles;
        public List<int> CompletedPuzzles => _completedPuzzles;
    }
}