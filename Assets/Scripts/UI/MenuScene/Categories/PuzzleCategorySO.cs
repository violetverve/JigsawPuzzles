using UnityEngine;

namespace JigsawPuzzles.UI.MenuScene.Categories
{
    [CreateAssetMenu(fileName = "PuzzleCategory", menuName = "Create SO/Puzzle Category")]
    public class PuzzleCategorySO : ScriptableObject
    {
        [SerializeField] private PuzzleCategory _category;
        [SerializeField] private Sprite _categoryImage;

        public PuzzleCategory Category => _category;
        public Sprite CategoryImage => _categoryImage;
    }
}
