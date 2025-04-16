using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using PuzzlePiece;

namespace PuzzleData.Save
{
    [Serializable]
    public class SnappableSave
    {
        private List<PieceSave> _pieceSaves;
        public List<PieceSave> PieceSaves => _pieceSaves;

        [JsonConstructor]
        public SnappableSave(List<PieceSave> pieceSaves)
        {
            _pieceSaves = pieceSaves;
        }

        public SnappableSave(ISnappable snappable)
        {
            _pieceSaves = new List<PieceSave>();

            snappable.Pieces.ForEach(piece => _pieceSaves.Add(new PieceSave(piece)));
        }
    }
}