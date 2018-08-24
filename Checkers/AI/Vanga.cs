using System;
using System.Collections.Generic;
using System.Threading;

namespace Checkers.AI
{
    internal class Vanga
    {
        private bool _isWhiteSide;
        private Desk _desk;

        public Vanga(bool isWhiteSide, Desk desk)
        {
            _isWhiteSide = isWhiteSide;
            _desk = desk;
        }

        public void GenerateMovePositions()
        {
            var cells = _desk.Cells;
            _desk.CheckIfNeedBeate();
            if (_desk.NeedBeat)
            {
                var cellPosition = SelectBestBeatCombination(_desk);
                _desk.Cells[cellPosition.Get_row() * _desk.Width + cellPosition.Get_column()].Click(true);
            }
            else
            {
                foreach (var cell in cells)
                {
                }
            }
        }

        private Cell.CellPosition SelectBestBeatCombination(Desk desk)
        {
            var selectedCell = desk.Get_selectedCell();
            if (selectedCell != null)
            {
                var allowedPositions = desk.AllowedPositions;
                if (allowedPositions.Count == 1)
                {
                    var cell = desk.Cells[allowedPositions[0].Get_row() * desk.Width + allowedPositions[0].Get_column()];
                    return cell.GetCellPosition();
                }
            }

            var battleCheckersPositions = desk.BattleCheckersPositions;
            var battleCeckersCells = new List<Cell>();
            foreach (var battleCheckerPosition in battleCheckersPositions)
            {
                var cell = desk.Cells[battleCheckerPosition.Get_row() * desk.Width + battleCheckerPosition.Get_column()];
                battleCeckersCells.Add(cell);
            }

            if (battleCeckersCells.Count == 1)
                return battleCeckersCells[0].GetCellPosition();

            var currentScore = 0;
            Cell.CellPosition currentCellPosition = null;
            foreach (var battleCeckersCell in battleCeckersCells)
            {
                var score = GetScore(desk, battleCeckersCell.GetCellPosition());
                if (currentCellPosition == null)
                {
                    currentScore = score;
                    currentCellPosition = battleCeckersCell.GetCellPosition();
                }
                else
                {
                    if (score <= currentScore) continue;
                    currentScore = score;
                    currentCellPosition = battleCeckersCell.GetCellPosition();
                }
            }

            return currentCellPosition;
        }

        private int GetScore(Desk loadedDesk, Cell.CellPosition loadedCheckerCellPosition)
        {
            var score = 0;
            var desk = new Desk(loadedDesk.ReturnDeskAsRawText());
            desk.StartBotSimulation();
            desk.CheckIfNeedBeate();
            if (!desk.NeedBeat) return score;
            var checkerCell = desk.Cells[loadedCheckerCellPosition.Get_row() * desk.Width + loadedCheckerCellPosition.Get_column()];

            var battleCells = checkerCell.GetBattleCells();
            Cell.CellPosition currentCellPosition = null;
            foreach (var battleCell in battleCells)
            {
                var selectedScore = GetScore(desk, checkerCell.GetCellPosition(), battleCell.GetCellPosition(), score);
                if (currentCellPosition != null && selectedScore <= score) continue;
                currentCellPosition = battleCell.GetCellPosition();
                score = selectedScore;
            }

            return score;
        }

        private int GetScore(Desk loadedDesk, Cell.CellPosition loadedCheckerCellPosition, Cell.CellPosition loadedNextCellPosition, int currentScore)
        {
            var score = currentScore;
            var desk = new Desk(loadedDesk.ReturnDeskAsRawText());
            desk.StartBotSimulation();
            var side = desk.CurrentWhiteTurn;
            var checkerCell = desk.Cells[loadedCheckerCellPosition.Get_row() * desk.Width + loadedCheckerCellPosition.Get_column()];
            var nextCell = desk.Cells[loadedNextCellPosition.Get_row() * desk.Width + loadedNextCellPosition.Get_column()];
            checkerCell.Click(false);
            nextCell.Click(false);
            score += GetCeckerScore(desk.Get_lastShotDownChecker(), side);
            while (side == desk.CurrentWhiteTurn)
            {
                var cellPosition = SelectBestBeatCombination(desk);
                desk.Cells[cellPosition.Get_row() * desk.Width + cellPosition.Get_column()].Click(false);
                score += GetCeckerScore(desk.Get_lastShotDownChecker(), side);
            }

            // beat if need and compute score
            desk.CheckIfNeedBeate();
            if (!desk.NeedBeat)
                return score;

//            while (desk.NeedBeat && side == desk.CurrentWhiteTurn)
//            {
//                var cellPosition = SelectBestBeatCombination(_desk);
//                desk.Cells[cellPosition.Get_row() * _desk.Width + cellPosition.Get_column()].Click(false);
//                score += GetCeckerScore(desk.Get_lastShotDownChecker());
//            }

            return score;
        }

        /*
         * Take quean: 14
         * Take checker: 4
         * Lost quean: -15
         * Lost checker: -6
         */
        private int GetCeckerScore(Checker checker, bool isWhiteSide)
        {
            return checker == null ? 0 : (checker.Get_isWhite() != isWhiteSide ? (checker.Is_Quean() ? 14 : 4) : (checker.Is_Quean() ? -15 : -6));
        }
    }
}