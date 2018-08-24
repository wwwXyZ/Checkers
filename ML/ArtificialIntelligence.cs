using System;
using Checkers;

namespace AI
{
    public class ArtificialIntelligence
    {
        private bool _isWhiteSide;
        private Desk _desk;

        public ArtificialIntelligence(bool isWhiteSide)
        {
            _isWhiteSide = isWhiteSide;
        }

        public Cell[] ComputeNextPosition()
        {
            var tree = new MonteCarloML(_isWhiteSide, _desk);
            var startCell = new Cell(null, null, null, null);
            var endCell = new Cell(null, null, null, null);
            return new[] {startCell, endCell};
        }

        public void SetDesk(Desk desk)
        {
            _desk = desk;
        }
    }
}