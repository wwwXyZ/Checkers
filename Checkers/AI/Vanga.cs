using System;
using System.Collections.Generic;

namespace Checkers.AI
{
    internal class Vanga
    {
        private bool _vangaMadeTurn;
        private readonly Desk _desk;

        public Vanga(Desk desk)
        {
            _desk = desk;
            _vangaMadeTurn = false;
        }

        public bool Get_vangaMadeTurn() => _vangaMadeTurn;

        public void GenerateMovePositions()
        {
            Console.WriteLine("Vanga: GenerateMovePositions");
            var cells = _desk.Cells;
            _desk.CheckIfNeedBeate();
            if (_desk.NeedBeat)
            {
                CellPosition cellPosition = null;
//                var thread = new Thread(() =>
//                {
                cellPosition = SelectBestBeatCombination(_desk);
//                }, 2000000000);
//                thread.Start();
//                thread.Join();
                if (cellPosition == null) return;
                Console.WriteLine($@"Vanga: Click = r:{cellPosition.Get_row()} c:{cellPosition.Get_column()}");
                _desk.GetCell(cellPosition).Click(true);
                _vangaMadeTurn = true;
            }
            else
            {
                foreach (var cell in cells)
                {
                }
            }
        }

        private CellPosition SelectBestBeatCombination(Desk desk)
        {
            var selectedCell = desk.Get_selectedCell();
            CellPosition currentCellPosition;
            int currentScore;
            if (selectedCell != null)
            {
                Console.WriteLine("Vanga nselectedCell != null");
                var selectedCellPosition = selectedCell.GetCellPosition();
                var allowedPositions = desk.AllowedPositions;
                currentScore = 0;
                currentCellPosition = null;
                foreach (var position in allowedPositions)
                {
                    var score = GetScore(desk, selectedCellPosition, position, 0);
                    if (currentCellPosition == null)
                    {
                        currentScore = score;
                        currentCellPosition = position;
                    }
                    else
                    {
                        if (score <= currentScore) continue;
                        currentScore = score;
                        currentCellPosition = position;
                    }
                }

                Console.WriteLine($@"Vanga nextCellPosition = {(currentCellPosition == null ? "null" : $"r:{currentCellPosition.Get_row()} c:{currentCellPosition.Get_column()}")}");
                return currentCellPosition;
            }

            var battleCheckersPositions = desk.BattleCheckersPositions;
            var battleCeckersCells = new List<Cell>();
            foreach (var battleCheckerPosition in battleCheckersPositions)
            {
                var cell = desk.GetCell(battleCheckerPosition);
                battleCeckersCells.Add(cell);
            }

            currentScore = 0;
            currentCellPosition = null;
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

        private int GetScore(Desk loadedDesk, CellPosition loadedCheckerCellPosition)
        {
            var score = 0;
            var desk = new Desk(loadedDesk.ReturnDeskAsRawText());
            desk.StartBotSimulation();
            desk.CheckIfNeedBeate();
            if (!desk.NeedBeat) return score;
            var checkerCell = desk.GetCell(loadedCheckerCellPosition);

            var battleCells = checkerCell.GetBattleCells();
            CellPosition currentCellPosition = null;
            foreach (var battleCell in battleCells)
            {
                var selectedScore = GetScore(desk, checkerCell.GetCellPosition(), battleCell.GetCellPosition(), score);
                if (currentCellPosition != null && selectedScore <= score) continue;
                currentCellPosition = battleCell.GetCellPosition();
                score = selectedScore;
            }

            return score;
        }

        private int GetScore(Desk loadedDesk, CellPosition loadedCheckerCellPosition, CellPosition loadedNextCellPosition, int currentScore)
        {
            var score = currentScore;
            var desk = new Desk(loadedDesk.ReturnDeskAsRawText());
            if (desk.Get_finishedGame()) return score;
            desk.StartBotSimulation();
            var side = desk.CurrentWhiteTurn;
            var checkerCell = desk.GetCell(loadedCheckerCellPosition);
            var nextCell = desk.GetCell(loadedNextCellPosition);
            checkerCell.Click(false);
            if (desk.Get_finishedGame()) return score;
            nextCell.Click(false);
            if (desk.Get_finishedGame()) return score;
            var lastShotDownCell = desk.Get_lastShotDownCheckerCell();
            if (lastShotDownCell == null) //debug
                throw new Exception("Muuuuu!");
            score += GetCeckerScore(lastShotDownCell.Checker, side);
            Cell currentShotDownCell;
            while (side == desk.CurrentWhiteTurn)
            {
                var cellPosition = SelectBestBeatCombination(desk);
                desk.GetCell(cellPosition).Click(false);
                if (desk.Get_finishedGame()) return score;
                currentShotDownCell = desk.Get_lastShotDownCheckerCell();
                if (!lastShotDownCell.GetCellPosition().Equals(currentShotDownCell.GetCellPosition()))
                    score += GetCeckerScore(currentShotDownCell.Checker, side);
            }

            // beat if need and compute score
            desk.CheckIfNeedBeate();
            if (!desk.NeedBeat)
                return score;


            var anotherSideDesk = new Desk(desk.ReturnDeskAsRawText());
            anotherSideDesk.StartBotSimulation();
            anotherSideDesk.CheckIfNeedBeate();
            if (!anotherSideDesk.NeedBeat) return score;
            while (side != anotherSideDesk.CurrentWhiteTurn)
            {
                var cellPosition = SelectBestBeatCombination(anotherSideDesk);
                anotherSideDesk.GetCell(cellPosition).Click(false);
                if (desk.Get_finishedGame()) return score;
                currentShotDownCell = anotherSideDesk.Get_lastShotDownCheckerCell();
                if (currentShotDownCell != null && !lastShotDownCell.GetCellPosition().Equals(currentShotDownCell.GetCellPosition()))
                    score += GetCeckerScore(currentShotDownCell.Checker, side);
            }

            currentShotDownCell = anotherSideDesk.Get_lastShotDownCheckerCell();
            if (!lastShotDownCell.GetCellPosition().Equals(currentShotDownCell.GetCellPosition()))
                score += GetCeckerScore(currentShotDownCell.Checker, side);
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
            var score = checker == null ? 0 : (checker.Get_isWhite() != isWhiteSide ? (checker.Is_Quean() ? 14 : 4) : (checker.Is_Quean() ? -15 : -6));
            return score;
        }
    }
}