using Firebase.Firestore;
using UnityEngine;
using JigsawPuzzles.UI.MenuScene.Categories;
using JigsawPuzzles.Services.Firebase.Firestore;
using System.Threading.Tasks;

namespace JigsawPuzzles.PuzzleData
{
    [FirestoreData]
    public class PuzzleDocument: IPuzzleData
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string PreviewImagePath { get; set; }
        [FirestoreProperty]
        public string FullSizeImagePath { get; set; }
        [FirestoreProperty]
        public bool IsLocked { get; set; }
        [FirestoreProperty]
        public PuzzleCategory Category { get; set; }
        [FirestoreProperty]
        public string PreviewImageURL { get; set; }
        [FirestoreProperty]
        public string FullSizeImageURL { get; set; }

        public Sprite PreviewSprite { get; private set; }
        public Sprite FullSizeSprite { get; private set; }


        public bool IsSecret => Category == PuzzleCategory.Secret;

        public void SetPreviewSprite(Sprite sprite)
        {
            PreviewSprite = sprite;
        }

        public void SetFullSizeSprite(Sprite sprite)
        {
            FullSizeSprite = sprite;
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public async Task LoadPreviewSprite()
        {
            if (PreviewSprite == null)
            {
                var sprite = await FirestoreManager.DownloadImageAsync(PreviewImageURL);

                if (sprite != null)
                {
                    SetPreviewSprite(sprite);

                    while (PreviewSprite == null)
                    {
                        await Task.Yield();
                    }
                }
            }
        }

        public async Task LoadFullSizeSprite()
        {
            if (FullSizeSprite == null)
            {
                var sprite = await FirestoreManager.DownloadImageAsync(FullSizeImageURL);

                SetFullSizeSprite(sprite);
            }
        }
    }

}
