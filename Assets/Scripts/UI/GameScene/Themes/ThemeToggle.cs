using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScene.Themes
{
    public class ThemeToggle : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Toggle _toggle;

        public Toggle Toggle => _toggle;

        public void Initialize(ToggleGroup toggleGroup, Color color)
        {
            _toggle.group = toggleGroup;
            SetColor(color);
        }
        
        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public Color GetColor()
        {
            return _image.color;
        }
    }
}