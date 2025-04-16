using UnityEngine;
using JigsawPuzzles.UI.MenuScene.Categories;
using JigsawPuzzles.PuzzleData;

namespace PuzzleData
{
    [CreateAssetMenu(fileName = "PuzzleSO", menuName = "Create SO/PuzzleSO")]
    public class PuzzleSO : ScriptableObject, IPuzzleData
    {
        [SerializeField] private int _id;
        [SerializeField] private Sprite _puzzleImage;
        [SerializeField] private Sprite _previewImage;
        [SerializeField] private bool _isLocked;
        [SerializeField] private PuzzleCategory _category;

        public int Id => _id;
        public Sprite PuzzleImage => _puzzleImage;
        public bool IsLocked => _isLocked;
        public bool IsSecret => _category == PuzzleCategory.Secret;
        public PuzzleCategory Category => _category;
        public Sprite FullSizeSprite => _puzzleImage;
        public Sprite PreviewSprite => _previewImage;

        public string FullSizeImagePath => "";
        public string PreviewImagePath => "";

        public void SetFullSizeSprite(Sprite sprite)
        {
            _puzzleImage = sprite;
        }
    }
}

