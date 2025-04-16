using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScene.Themes
{
    public class ThemesGrid : MonoBehaviour
    {
        [SerializeField] private ThemesSO _themesSO;
        [SerializeField] private ThemeToggle _themeTogglePrefab;
        [SerializeField] private ToggleGroup _toggleGroup;
        private List<ThemeToggle> _toggles = new List<ThemeToggle>();
        
        public List<ThemeToggle> Toggles => _toggles;
        
        private void Awake()
        {
            foreach (var color in _themesSO.Themes)
            {
                ThemeToggle themeToggle = Instantiate(_themeTogglePrefab, transform);
                themeToggle.Initialize(_toggleGroup, color);
                _toggles.Add(themeToggle);
            }
        }
    }
}

