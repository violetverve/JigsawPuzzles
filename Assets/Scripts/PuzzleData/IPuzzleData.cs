using UnityEngine;
using JigsawPuzzles.UI.MenuScene.Categories;

namespace JigsawPuzzles.PuzzleData
{
    public interface IPuzzleData
    {
        int Id { get; }
        bool IsLocked { get; }
        PuzzleCategory Category { get; }
        Sprite PreviewSprite { get; }
        Sprite FullSizeSprite { get; }
        string FullSizeImagePath { get; }
        string PreviewImagePath { get; }

        bool IsSecret => Category == PuzzleCategory.Secret;
        void SetFullSizeSprite(Sprite sprite);
    }
}
