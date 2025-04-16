using PuzzleData;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

    [CreateAssetMenu(fileName = "PuzzlesList", menuName = "Create SO/Puzzles List")]
    public class PuzzleList : ScriptableObject
    {
        [SerializeField] private List<PuzzleSO> _puzzleList = new();

        public List<PuzzleSO> List => _puzzleList;

        public PuzzleSO GetPuzzleByID(int id)
        {
            return _puzzleList.FirstOrDefault(puzzle => puzzle.Id == id);
        }
    }


