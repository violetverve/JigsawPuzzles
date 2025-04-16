using System.Collections.Generic;
using UnityEngine;

namespace UI.MenuScene
{
    [CreateAssetMenu(fileName = "ColorPaletteSO", menuName = "Create SO/ColorPalette", order = 1)]
    public class ColorPaletteSO : ScriptableObject
    {
        [SerializeField] private List<Color> _colors;

        public List<Color> Colors => _colors;

        public Color GetColor(int index)
        {
            if (_colors == null || _colors.Count == 0)
            {
                Debug.LogWarning("Color list is empty or not initialized.");
                return Color.white;
            }

            int colorIndex = index % _colors.Count;
            return _colors[colorIndex];
        }
    }
}