using System.Collections.Generic;
using UnityEngine;

namespace UI.GameScene.Themes
{
    [CreateAssetMenu(menuName = "Themes/ThemesSO")]
    public class ThemesSO : ScriptableObject
    {
        [SerializeField] private List<Color> _themes;

        public List<Color> Themes => _themes;
    }
}

