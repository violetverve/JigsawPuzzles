using JigsawPuzzles.UI.MenuScene.Categories;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MenuScene.Puzzle
{
    public class SecretLevel : MonoBehaviour
    {
        [SerializeField] private Image _stripes;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private ColorPaletteSO _backgroundPalette;
        [SerializeField] private ColorPaletteSO _stripesPalette;

        public void LoadSecretLevel(bool isSecret, int puzzleID)
        {
            gameObject.SetActive(isSecret);
            if (isSecret)
            {
                _backgroundImage.sprite = null;
                _stripes.color = _stripesPalette.GetColor(GetSecretLevelID(puzzleID));
                _backgroundImage.color = _backgroundPalette.GetColor(GetSecretLevelID(puzzleID));
            }
        }

        private int GetSecretLevelID(int puzzleID)
        {
            return (puzzleID - 1000) / ((int)PuzzleCategory.Secret + 1);
        }

    }
}