using UnityEngine;
using UnityEngine.SceneManagement;
using JigsawPuzzles.PuzzleData;

namespace JigsawPuzzles.UI.LoadScene
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        private void OnEnable()
        {
            LoadBar.LoadCompleted += HandleLoadCompleted;
            DocumentLoader.LoadCompleted += HandleLoadCompleted;
        }

        private void OnDisable()
        {
            LoadBar.LoadCompleted -= HandleLoadCompleted;
            DocumentLoader.LoadCompleted -= HandleLoadCompleted;
        }

        private void HandleLoadCompleted()
        {
            LoadScene();
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(_sceneName);
        }
    }

}
