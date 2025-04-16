using System.Collections.Generic;
using UnityEngine;
using Grid;

namespace GameManagement.Difficulty
{
    [CreateAssetMenu(menuName = "Difficulty/DifficultyManagerSO")]
    public class DifficultyManagerSO : ScriptableObject
    {
        [SerializeField] private List<DifficultySO> _difficulties;

        [SerializeField] private float _rotationBonusPercentage;

        public List<DifficultySO> Difficulties => _difficulties;
        public float RotationBonusPercentage => _rotationBonusPercentage;

        public DifficultySO GetDifficulty(int index)
        {
            return _difficulties[index];
        }

        public GridSO GetGridSOBySide(int side)
        {
            return _difficulties.Find(difficulty => difficulty.Grid.Width == side).Grid; 
        }

        public int GetRewardByGridSO(GridSO gridSO)
        {
            return _difficulties.Find(difficulty => difficulty.Grid == gridSO).Reward;
        }

    }
}