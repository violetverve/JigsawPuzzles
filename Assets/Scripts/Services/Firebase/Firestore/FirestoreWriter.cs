using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using JigsawPuzzles.UI.MenuScene.Categories;
using UnityEngine;
using System.Text.RegularExpressions;
using Firebase.Storage;

namespace JigsawPuzzles.Services.Firebase.Firestore
{
    public class FirestoreWriter : MonoBehaviour
    {
        private FirebaseFirestore _db;
        [SerializeField] private string _imagesFolderPath; // Local folder

        [SerializeField] private string[] _categoriesLocalPaths;
        [SerializeField] private int[] _categoriesFirstUnlockIds;

        [SerializeField] private int puzzleId;
        [SerializeField] private PuzzleCategory puzzleCategory;

        [SerializeField] private bool _deleteAllDocuments;
        [SerializeField] private bool _writeAllDocuments;

        [SerializeField] private int _unlockBlockSize = 5;

        int _unlockedPuzzles = 0;   


        private void Awake()
        {
            _db = FirebaseFirestore.DefaultInstance;


            if (_deleteAllDocuments)
            {
                _ = DeleteAllDocumentsAsync("levels");
            }


            if (_writeAllDocuments)
            {
                WriteAllCategoriesDocuments();

            }

            Debug.Log($"Unlocked puzzles: {_unlockedPuzzles}");
        }

        public void WriteAllCategoriesDocuments()
        {
            foreach (PuzzleCategory category in Enum.GetValues(typeof(PuzzleCategory)))
            {
                WriteAllPuzzleDocuments(category, _categoriesLocalPaths[(int)category]);
            }
        }

        public void WriteAllPuzzleDocuments(PuzzleCategory puzzleCategory, string localFolderPath)
        {
            this.puzzleCategory = puzzleCategory;
            string[] filePaths = Directory.GetFiles(localFolderPath);
            int id = puzzleId + (int)puzzleCategory + 1;
            List<string> fileNames = new List<string>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                fileNames.Add(fileName);
            }
            fileNames = fileNames.OrderBy(f =>
                int.Parse(Regex.Match(f, @"^\d+").Value)).ToList();

            int inCategoryPuzzleId = 0;
            foreach (string fileName in fileNames)
            {
                bool isUnlocked = IfPuzzleUnlocked(inCategoryPuzzleId, (int)puzzleCategory);

                WritePuzzleDocument(id, fileName, !isUnlocked);
                id += 12;
                inCategoryPuzzleId++;

                if (isUnlocked)
                {
                    _unlockedPuzzles++;
                }
            }
        }


        public async void WritePuzzleDocument(int id, string imageFileName, bool isLocked)
        {


            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Id", id },
                { "PreviewImagePath", puzzleCategory.ToString().ToLower() + "/previews/" + imageFileName },
                { "FullSizeImagePath", puzzleCategory.ToString().ToLower() + "/fullsize/" + imageFileName },
                { "PreviewImageURL", null },
                { "FullSizeImageURL", null},
                { "IsLocked", isLocked },
                { "Category", puzzleCategory }
            };

            // get download URL

            var previewImageURL = await GetDownloadUrlAsync(data["PreviewImagePath"].ToString());
            var fullSizeImageURL = await GetDownloadUrlAsync(data["FullSizeImagePath"].ToString());

            data["PreviewImageURL"] = previewImageURL;
            data["FullSizeImageURL"] = fullSizeImageURL;


            Debug.Log($"Writing document: {id} - {puzzleCategory} - {imageFileName}");

            _ = WriteDocumentAsync("levels", id.ToString(), data);
        }


        public async Task WriteDocumentAsync(string collection, string documentId, Dictionary<string, object> data)
        {
            try
            {
                DocumentReference docRef;
                if (string.IsNullOrEmpty(documentId))
                {
                    // Auto-generate a document ID
                    docRef = _db.Collection(collection).Document();
                }
                else
                {
                    docRef = _db.Collection(collection).Document(documentId);
                }

                await docRef.SetAsync(data);
                Debug.Log($"Document successfully written! ID: {docRef.Id}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to write document: {ex.Message}\n{ex.StackTrace}");
            }
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

        public async Task DeleteAllDocumentsAsync(string collection)
        {
            try
            {
                CollectionReference collectionRef = _db.Collection(collection);
                QuerySnapshot snapshot = await collectionRef.GetSnapshotAsync();

                List<Task> deleteTasks = new List<Task>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    deleteTasks.Add(document.Reference.DeleteAsync());
                    Debug.Log($"Deleting document: {document.Id}");
                }

                await Task.WhenAll(deleteTasks);
                Debug.Log($"All documents in '{collection}' collection have been deleted.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete documents: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public async Task UpdateDocumentAsync(string collection, string documentId, Dictionary<string, object> updates)
        {
            try
            {
                DocumentReference docRef = _db.Collection(collection).Document(documentId);
                await docRef.UpdateAsync(updates);
                Debug.Log($"Document {documentId} successfully updated!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update document: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public bool IfPuzzleUnlocked(int inCategoryPuzzleId, int categoryId)
        {
            if (categoryId == (int)PuzzleCategory.Secret)
            {
                return false;
            }

            if (inCategoryPuzzleId >= _categoriesFirstUnlockIds[categoryId])
            {
                if ((inCategoryPuzzleId - _categoriesFirstUnlockIds[categoryId] ) % _unlockBlockSize == 0)
                {
                    return true;
                }
                return false;
            }

            return false;
        }
    }
}
