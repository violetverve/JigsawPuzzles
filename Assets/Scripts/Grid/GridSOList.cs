using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [CreateAssetMenu(menuName = "Grid/GridSOList")]
    public class GridSOList : ScriptableObject
    {
        [SerializeField] private List<GridSO> _gridSOList;

        public List<GridSO> GridDiffucultiesList => _gridSOList;
    }
}

