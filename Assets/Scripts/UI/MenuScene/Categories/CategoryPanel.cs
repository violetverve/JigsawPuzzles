using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace JigsawPuzzles.UI.MenuScene.Categories
{
    public class CategoryPanel : MonoBehaviour
    {
        [SerializeField] private Image _flagImage;
        [SerializeField] private TextMeshProUGUI _categoryName;

        [SerializeField] private Image _categoryImage;
        private PuzzleCategory _category;

        public static event Action<PuzzleCategory> CategorySelected;

        public void Initialize(PuzzleCategorySO categorySO, Color color)
        {
            _categoryImage.sprite = categorySO.CategoryImage;
            _categoryName.text = categorySO.Category.ToString();

            _flagImage.color = color;
            _category = categorySO.Category;
        }

        public void OnCategorySelected()
        {
            CategorySelected?.Invoke(_category);
        }
    }
}