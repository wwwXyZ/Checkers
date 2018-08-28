using System;
using System.Collections.Generic;
using System.Threading;

namespace Checkers.AI
{
    public class ArtificialIntelligence
    {
        private readonly bool _isWhiteSide;
        private Desk _desk;

        public bool Get_isWhiteSide() => _isWhiteSide;

        public ArtificialIntelligence(bool isWhiteSide) => _isWhiteSide = isWhiteSide;

        public void MoveToNextPosition()
        {
            var vanga = new Vanga(_desk);
            vanga.GenerateMovePositions();
            if (vanga.Get_vangaMadeTurn()) return;
            //select first combination
//            var selectedCell = _desk.Get_selectedCell();
//            if (selectedCell != null)
//            {
//                var position = selectedCell.GetAllowedPositions()[0];
//                _desk.GetCell(position).Click(true);
//            }
//            foreach (var cell in _desk.Cells)
//            {
//                if(cell.Checker == null || cell.Checker.Get_isWhite() != _isWhiteSide) continue;
//                var allowedPositions = cell.GetAllowedPositions();
//                if (allowedPositions.Count <= 0) continue;
//                cell.Click(true);
//                break;
//            }
            //random turn
            var selectedCell = _desk.Get_selectedCell();
            if (selectedCell != null)
            {
                var allowedPositions = selectedCell.GetAllowedPositions();
                if (allowedPositions.Count == 0) return;
                _desk.GetCell(allowedPositions[new Random().Next(allowedPositions.Count)]).Click(true);
                if (_desk.Get_finishedGame()) return;
            }

            var checkersCells = new List<Cell>();
            foreach (var cell in _desk.Cells)
            {
                if (cell.Checker == null || cell.Checker.Get_isWhite() != _isWhiteSide) continue;
                var allowedPositions = cell.GetAllowedPositions();
                if (allowedPositions.Count <= 0) continue;
                checkersCells.Add(cell);
                break;
            }

            if (checkersCells.Count > 0)
                checkersCells[new Random().Next(checkersCells.Count)].Click(true);

            //            var tree = new MonteCarloMl(_isWhiteSide, _desk);
        }

        public void SetDesk(Desk desk) => _desk = desk;
    }
}