using JigsawPuzzles.Services.Firebase.Firestore;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace JigsawPuzzles.PuzzleData
{
    public class DocumentLoader : MonoBehaviour
    {
        public static DocumentLoader Instance { get; private set; }

        [SerializeField] private int _initialPreviewBatchSize = 16;
        [SerializeField] private int _initialDocumentBatchSize = 16;

        private List<PuzzleDocument> _puzzleDocuments = new List<PuzzleDocument>();
        private List<PuzzleDocument> _puzzleDocumentsInProgress = new List<PuzzleDocument>();
        public List<PuzzleDocument> PuzzleDocuments => _puzzleDocuments;
        public List<PuzzleDocument> PuzzleDocumentsInProgress => _puzzleDocumentsInProgress;

        public static event Action LoadCompleted;
        public event Action<float> LoadProgress;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartCoroutine(LoadInitialBatch());
        }

        private IEnumerator LoadInitialBatch()
        {
            float start = Time.time;

            yield return LoadPuzzleDocuments(_initialDocumentBatchSize, true);

            LoadProgress?.Invoke(0.25f);

            yield return LoadPreviewBatch(_initialPreviewBatchSize);

            LoadProgress?.Invoke(1f);

            LoadCompleted?.Invoke();

            Debug.Log($"LoadInitialBatch() took {Time.time - start} seconds");
        }

        private IEnumerator LoadPreviewBatch(int batchSize)
        {
            List<Task> loadTasks = new List<Task>();

            int previews = 0;

            foreach (PuzzleDocument document in _puzzleDocuments)
            {
                if (!PlayerData.Instance.IsPuzzleCompleted(document.Id))
                {
                    loadTasks.Add(document.LoadPreviewSprite());
                    previews++;
                }

                if (previews >= batchSize)
                {
                    break;
                }
            }

            yield return new WaitUntil(() => loadTasks.All(t => t.IsCompleted));
        }


        private IEnumerator LoadPuzzleDocuments(int batchSize, bool all = false)
        {
            Task<List<PuzzleDocument>> loadTask = FirestoreManager.LoadPuzzleDocumentsBatch(batchSize, all);
            yield return new WaitUntil(() => loadTask.IsCompleted);
            if (loadTask.Exception != null)
            {
                Debug.LogError("Failed to load puzzles: " + loadTask.Exception);
                yield break;
            }
            if (loadTask.Result.Count == 0)
            {
                Debug.Log("No DOCUMENTS to load");
                yield break;
            }
            _puzzleDocuments.AddRange(loadTask.Result);
        }
    }
}

