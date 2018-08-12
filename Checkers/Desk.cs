using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using FontFamily = Svg.Text.FontFamily;

namespace Checkers
{
    public class Desk
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private static readonly int MinWidth;
        private static readonly int MinHeight;
        private static readonly int DefaultWidth;
        private static readonly int DefaultHeight;

        public bool WhiteOnTop { get; }
        public bool CurrentWhiteTurn;
        public int Rotation { get; } // 0 - 0 degree; 1 - 90 degree; 2 - 180 degree; 3 - 270 degree
        private int _blackCount;
        private int _whiteCount;
        public bool NeedBeat { get; set; }

        private Cell _selectedCell;
        public List<Cell> Cells = new List<Cell>();
        public List<Cell.CellPosition> TopDefaultPositions = new List<Cell.CellPosition>();
        public List<Cell.CellPosition> BottomDefaultPositions = new List<Cell.CellPosition>();
        public List<Cell.CellPosition> AllowedPositions = new List<Cell.CellPosition>();
        public List<Cell.CellPosition> BattleCheckerPositions = new List<Cell.CellPosition>();

        public int Get_blackCount()
        {
            return _blackCount;
        }

        public void Set_blackCount(int value)
        {
            _blackCount = value;
        }

        public int Get_whiteCount()
        {
            return _whiteCount;
        }

        public void Set_whiteCount(int value)
        {
            _whiteCount = value;
        }

        public Cell Get_selectedCell()
        {
            return _selectedCell;
        }

        public static int GetMinHeight()
        {
            return MinHeight;
        }

        public static int GetMinWidth()
        {
            return MinWidth;
        }

        static Desk()
        {
            MinWidth = 8;
            MinHeight = 8;
            DefaultWidth = 8;
            DefaultHeight = 8;
        }

        public Desk(int width, int height, bool whiteOnTop, int rotation, bool currentWhiteTurn)
        {
            Width = width > MinWidth ? width : MinWidth;
            Height = height > MinHeight ? height : MinHeight;
            WhiteOnTop = whiteOnTop;
            Rotation = rotation;
            CurrentWhiteTurn = currentWhiteTurn;
            OrganizeDefaultPositions();
        }

        public Desk()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            WhiteOnTop = true;
            Rotation = 0;
            CurrentWhiteTurn = true;
            OrganizeDefaultPositions();
        }

        public void OrganizeDefaultPositions()
        {
            TopDefaultPositions.Clear();
            BottomDefaultPositions.Clear();

            for (var column = 0; column < Width; column++)
            {
                for (var row = 0; row < Height; row++)
                {
                    if ((column + row) % 2 != 0) continue;
                    if (row < Width / 2 - 1)
                        TopDefaultPositions.Add(new Cell.CellPosition(column, row));
                    else if (row >= Width / 2 + 1)
                        BottomDefaultPositions.Add(new Cell.CellPosition(column, row));
                }
            }
        }

        private void Add_cell(Cell cell)
        {
            if (cell.Checker != null)
                if (cell.Checker.Get_isWhite())
                    Set_whiteCount(_whiteCount + 1);
                else
                    Set_blackCount(_blackCount + 1);
            Cells.Add(cell);
        }

        public void Clear_cells()
        {
            Set_blackCount(0);
            Set_whiteCount(0);
            Cells.Clear();
        }

        private Cell ConstructCell(int row, int column, bool withCheckers)
        {
            var cellColor = new Cell.CellColor(((column + row) % 2) != 0);
            var cellPosition = new Cell.CellPosition(column, row);
            var deskCell = new Cell(cellPosition, cellColor, null, this);
            if (withCheckers)
                deskCell.SetDefaultChecker(this);
            return deskCell;
        }

        public void Generate(bool withCeckers)
        {
            switch (Rotation)
            {
                case 1: // right
                    for (var column = 0; column < Width; column++)
                    {
                        for (var row = Height - 1; row >= 0; row--)
                        {
                            Add_cell(ConstructCell(row, column, withCeckers));
                        }
                    }

                    break;
                case 2: // invert
                    for (var row = Height - 1; row >= 0; row--)
                    {
                        for (var column = Width - 1; column >= 0; column--)
                        {
                            Add_cell(ConstructCell(row, column, withCeckers));
                        }
                    }

                    break;
                case 3: // left
                    for (var column = Width - 1; column >= 0; column--)
                    {
                        for (var row = 0; row < Height; row++)
                        {
                            Add_cell(ConstructCell(row, column, withCeckers));
                        }
                    }

                    break;
                default: // 0 or default
                    for (var row = 0; row < Height; row++)
                    {
                        for (var column = 0; column < Width; column++)
                        {
                            Add_cell(ConstructCell(row, column, withCeckers));
                        }
                    }

                    break;
            }
        }

        public void ShowAllowedPosition(Cell cell)
        {
            var checker = cell.Checker;
            if (!checker.Is_Quean())
            {
                var neighbors = cell.GetCheckerNeighborsList(true);
                foreach (var neighbordCell in neighbors)
                {
                    if (neighbordCell.Checker == null)
                        AllowedPositions.Add(neighbordCell.GetCellPosition());
                }
            }
            else
            {
                var diagonals = cell.GetCellDiagonals();
                foreach (var diagonal in diagonals)
                foreach (var diagonalCell in diagonal.Cells)
                {
                    if (diagonalCell.Checker != null) break;
                    AllowedPositions.Add(diagonalCell.GetCellPosition());
                }
            }
        }

        public void UnselectLastCell()
        {
            AllowedPositions.Clear();
            _selectedCell = null;
        }

        public void ReRenderTable()
        {
            ((MainWindow) Application.Current.MainWindow)?.RenderBattlefield(false);
        }

        public void SetSelectedCell(Cell cell)
        {
            _selectedCell = cell;
        }

        public void CheckIfNeedBeate()
        {
            NeedBeat = false;
            foreach (var cell in Cells)
            {
                var battleCells = cell.GetBattleCells();
                if (battleCells.Count == 0) continue;
                NeedBeat = true;
            }
        }

        public void CheckIfNeedBeate(Cell cell)
        {
            NeedBeat = false;

            var battleCells = cell.GetBattleCells();
            NeedBeat = battleCells.Count > 0;
        }
    }
}