using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Checkers.AI
{
    public class ArtificialIntelligence
    {
        private static int _seed = 4886;

        private readonly bool _isWhiteSide;
        private Desk _desk;

        public bool Get_isWhiteSide() => _isWhiteSide;

        public ArtificialIntelligence(bool isWhiteSide) => _isWhiteSide = isWhiteSide;

        public void MoveToNextPosition()
        {
            Console.WriteLine("Start AI");
            var vanga = new Vanga(_desk);
            Console.WriteLine("Vanga created");
            vanga.GenerateMovePositions();
            if (vanga.Get_vangaMadeTurn()) return;
            Console.WriteLine("Vanga nothing click");
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
            Console.WriteLine("Random start");
            var selectedCell = _desk.Get_selectedCell();
            if (selectedCell != null)
            {
                Console.WriteLine($@"Random selected cell = {(selectedCell.GetCellPosition() == null ? "null" : $"r:{selectedCell.GetCellPosition().Get_row()} c:{selectedCell.GetCellPosition().Get_column()}")}");
                var allowedPositions = selectedCell.GetAllowedPositions();
                Console.WriteLine($@"allowedPositions.Count = {allowedPositions.Count}");
                if (allowedPositions.Count == 0)
                    return;
                var randNumber = new Random(_seed).Next(allowedPositions.Count);
                Console.WriteLine($@"randNumber = {randNumber}");
                var nextPosition = allowedPositions[randNumber];
                Console.WriteLine($@"nextPosition = r:{nextPosition.Get_row()} c:{nextPosition.Get_column()}");
                _desk.GetCell(nextPosition).Click(true);
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
                checkersCells[new Random(_seed).Next(checkersCells.Count)].Click(true);

            //            var tree = new MonteCarloMl(_isWhiteSide, _desk);
        }

        public void SetDesk(Desk desk) => _desk = desk;

        public static void IncrementSeed()
        {
            Console.Clear();
//            _seed = new Random().Next(0, 10000);
            Console.WriteLine($@"!!!!!!!!!!{_seed}!!!!!!!!!!");
        }
    }
}