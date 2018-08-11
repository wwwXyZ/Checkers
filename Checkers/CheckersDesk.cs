using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Checkers
{
    public class CheckersDesk
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private static readonly int MinWidth;
        private static readonly int MinHeight;
        private static readonly int DefaultWidth;
        public bool WhiteOnTop { get; }
        public bool CurrentWhiteTurn;
        public int Rotation { get; } // 0 - default; 1 - right; 2 - invert; 3 - left
        public bool NeedBeat { get; set; }
        private static readonly int DefaultHeight;
        private DeskCell _selectedCell;
        public List<DeskCell> Cells = new List<DeskCell>();
        public List<DeskCell.DeskCellPosition> TopDefaultPositions = new List<DeskCell.DeskCellPosition>();
        public List<DeskCell.DeskCellPosition> BottomDefaultPositions = new List<DeskCell.DeskCellPosition>();
        public List<DeskCell.DeskCellPosition> AllowedPositions = new List<DeskCell.DeskCellPosition>();

        public DeskCell Get_selectedCell()
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

        static CheckersDesk()
        {
            MinWidth = 8;
            MinHeight = 8;
            DefaultWidth = 8;
            DefaultHeight = 8;
        }


        public CheckersDesk(int width, int height, bool whiteOnTop, int rotation, bool currentWhiteTurn)
        {
            Width = width > MinWidth ? width : MinWidth;
            Height = height > MinHeight ? height : MinHeight;
            WhiteOnTop = whiteOnTop;
            Rotation = rotation;
            CurrentWhiteTurn = currentWhiteTurn;
            Update_cellsPartCount();
        }

        public CheckersDesk()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            WhiteOnTop = true;
            Rotation = 0;
            CurrentWhiteTurn = true;
            Update_cellsPartCount();
        }

        private void Update_cellsPartCount()
        {
            // Half of Table cells only on black
//            _cellsPartCount = ((Height * Width) / 2 / 2) - Width / 2;
            GetDefaultPositions();
        }

        public void GetDefaultPositions()
        {
            TopDefaultPositions.Clear();
            BottomDefaultPositions.Clear();
            for (var column = 0; column < Width; column++)
            {
                for (var row = 0; row < Height; row++)
                {
                    if ((column + row) % 2 != 0) continue;
                    if (row < Width / 2 - 1)
                        TopDefaultPositions.Add(new DeskCell.DeskCellPosition(column, row));
                    else if (row >= Width / 2 + 1)
                        BottomDefaultPositions.Add(new DeskCell.DeskCellPosition(column, row));
                }
            }
        }

        private void Add_cell(DeskCell cell)
        {
            Cells.Add(cell);
        }

        public void Clear_cells()
        {
            Cells.Clear();
        }

        private DeskCell ConstructCell(int row, int column, bool withCheckers)
        {
            var descCellColor = new DeskCell.DeskCellColor(((column + row) % 2) != 0);
            var descCellPosition = new DeskCell.DeskCellPosition(column, row);
            var deskCell = new DeskCell(descCellPosition, descCellColor, null, this);
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

        public void ShowAllowedPosition(DeskCell cell)
        {
            var checker = cell.Checker;
            if (!checker.Is_king())
            {
                var neighbors = cell.GetCheckerNeighborsList(true);
                foreach (var neighbordCell in neighbors)
                {
                    var position = neighbordCell.GetCellPosition();
                    if (neighbordCell.Checker == null)
                        AllowedPositions.Add(position);
                }
            }
            else
            {
            }
        }

        public void UnselectLastCell()
        {
            AllowedPositions.Clear();
            _selectedCell = null;
        }

        public void ReRenderTable()
        {
            ((MainWindow) Application.Current.MainWindow)?.Render_checkers_position(false);
        }

        public void SetSelectedCell(DeskCell cell)
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
                break;
            }
        }
    }
}