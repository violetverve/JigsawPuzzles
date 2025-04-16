using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace JigsawPuzzles.UI.MenuScene.Tabs
{
    public class ButtonTMP : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private Color _interactableColor;
        private Color _nonInteractableColor;

        public static event Action<int> TabSelected;

        private void Awake()
        {
            _interactableColor = _button.colors.normalColor;
            _nonInteractableColor = _button.colors.disabledColor;
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _textMeshProUGUI.color = isInteractable ? _interactableColor : _nonInteractableColor;
            _image.color = isInteractable ? _interactableColor : _nonInteractableColor;
        }

        public void ButtonClicked()
        {
            SetInteractable(false);

            object selectedTabIndex = transform.GetSiblingIndex();

            TabSelected?.Invoke((int)selectedTabIndex);
        }

    }
}
