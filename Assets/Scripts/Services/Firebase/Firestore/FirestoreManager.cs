using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using JigsawPuzzles.PuzzleData;
using Firebase.Storage;
using UnityEngine.Networking;

namespace JigsawPuzzles.Services.Firebase.Firestore
{
    public static class FirestoreManager
    {
        public static async Task<List<PuzzleDocument>> LoadPuzzleDocumentsBatch(int limit, bool all = false)
        {
            List<PuzzleDocument> loadedPuzzles = new List<PuzzleDocument>();
            var db = FirebaseFirestore.DefaultInstance;
            var collectionRef = db.Collection("levels");

            try
            {

                Debug.Log("Loading documents");

                Query query = collectionRef;

                if (!all)
                {
                    query = collectionRef.Limit(limit);
                }

                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot document in querySnapshot.Documents)
                {
                    if (!document.Exists) continue;

                    PuzzleDocument puzzle = document.ConvertTo<PuzzleDocument>();

                    loadedPuzzles.Add(puzzle);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading Firestore documents: {ex.Message}\n{ex.StackTrace}");

                return new List<PuzzleDocument>();
            }

            return loadedPuzzles;
        }


        public static async Task<string> GetDownloadUrlAsync(string filePath)
        {
            try
            {
                StorageReference storageReference = FirebaseStorage.DefaultInstance.GetReference(filePath);
                Uri downloadUrl = await storageReference.GetDownloadUrlAsync();
                return downloadUrl.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting download URL: {ex.Message}; File path: {filePath}");
                return null;
            }
        }

        public static async Task<Sprite> DownloadImageAsync(string downloadUrl)
        {
            if (string.IsNullOrEmpty(downloadUrl))
            {
                Debug.LogError("Download URL is null or empty.");
                return null;
            }

            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(downloadUrl);
            request.SendWebRequest();

            while (!request.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError($"Error downloading image: {request.error}");
                return null;
            }
        }

        public static async Task<Sprite> DownloadImageAndGetSprite(string filePath)
        {
            string downloadUrl = await GetDownloadUrlAsync(filePath);

            if (string.IsNullOrEmpty(downloadUrl))
            {
                return null;
            }
            return await DownloadImageAsync(downloadUrl);
        }



    }
}
