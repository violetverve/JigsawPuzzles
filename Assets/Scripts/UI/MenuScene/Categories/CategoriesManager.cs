using System.Collections.Generic;
using UnityEngine;
using UI.MenuScene;
using TMPro;
using JigsawPuzzles.PuzzleData;

namespace JigsawPuzzles.UI.MenuScene.Categories
{
    public class CategoriesManager : MonoBehaviour
    {
        [Header("Category Settings")]
        [SerializeField] private List<PuzzleCategorySO> _categories;
        [SerializeField] private CategoryPanel _categoryPanelPrefab;
        [SerializeField] private Transform _categoriesParent;
        [SerializeField] private ColorPaletteSO _colorPalette;

        [Header("Puzzle Panels Settings")]
        [SerializeField] private CategoryWindow _categoryWindow;
        [SerializeField] private TextMeshProUGUI _categoryName;

        [Header("Scroll Manager")]
        [SerializeField] private ScrollManager _scrollManager;

        private void Awake()
        {
            InitializeCategories();
        }

        private void OnEnable()
        {
            CategoryPanel.CategorySelected += HandleCategorySelected;
            UnlockPuzzlePopUp.PuzzleUnlocked += HandlePuzzleUnlocked;
        }

        private void OnDisable()
        {
            CategoryPanel.CategorySelected -= HandleCategorySelected;
            UnlockPuzzlePopUp.PuzzleUnlocked -= HandlePuzzleUnlocked;
        }

        private void InitializeCategories()
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                var category = _categories[i];
                Color color = _colorPalette.GetColor(i);
                CategoryPanel categoryPanel = Instantiate(_categoryPanelPrefab, _categoriesParent);
                categoryPanel.Initialize(category, color);
            }
        }

        #region Event Handlers
        private void HandlePuzzleUnlocked(int puzzleId)
        {
            _scrollManager.UpdatePuzzlePanel(puzzleId);
        }

        private void HandleCategorySelected(PuzzleCategory category)
        {
            var categoryDocuments = GetPuzzlesByCategory(category);
            _categoryWindow.SetActive(true);

            _scrollManager.Initialize(categoryDocuments);

            _categoryName.text = category.ToString();
        }
        #endregion

        private List<PuzzleDocument> GetPuzzlesByCategory(PuzzleCategory category)
        {
            var puzzlesInCategory = DocumentLoader.Instance.PuzzleDocuments.FindAll(p => p.Category == category);
            puzzlesInCategory.Sort((p1, p2) => p1.Id.CompareTo(p2.Id));
            return puzzlesInCategory;
        }
    }
}

