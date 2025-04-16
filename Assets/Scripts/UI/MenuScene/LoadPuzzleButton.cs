using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JigsawPuzzles.UI.MenuScene
{
    public class LoadPuzzleButton : MonoBehaviour
    {
        [SerializeField] private LoadingCircle _loadingCircle;
        [SerializeField] private TextMeshProUGUI _buttonEnabledText;
        [SerializeField] private Button _startLevelButton;

        public void StartLoading()
        {
            _loadingCircle.gameObject.SetActive(true);
            _buttonEnabledText.gameObject.SetActive(false);
            _startLevelButton.interactable = false;
        }

        public void StopLoading()
        {
            _loadingCircle.gameObject.SetActive(false);
            _buttonEnabledText.gameObject.SetActive(true);
            _startLevelButton.interactable = true;
        }

    }
}

