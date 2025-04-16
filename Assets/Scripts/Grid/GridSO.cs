using UnityEngine;

namespace Grid
{
    [CreateAssetMenu(menuName = "Grid/GridSO")]
    [System.Serializable]
    public class GridSO : ScriptableObject
    {
        [SerializeField, Min(2)] private int _width;
        [SerializeField, Min(2)] private int _height;
        
        public int Width => _width;
        public int Height => _height;
        public int Area => _width * _height;
        public int Edges => (_width * 2) + (_height * 2) - 4;
    }
}