using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Checkers
{
    public partial class Cell
    {
        private readonly CellColor _color;
        private readonly CellPosition _position;
        public Checker Checker;
        public SolidColorBrush ActiveButtonColor;
        public SolidColorBrush ActiveCheckerColor;
        public SolidColorBrush ActiveKingCheckerColor;
        public SolidColorBrush AllowedPositionColor;
        public Button Button;
        private readonly Desk _desk;
        public bool Tmp;

        public Cell(CellPosition position, CellColor color, Checker checker, Desk desk)
        {
            _position = position;
            _color = color;
            Checker = checker;
            ActiveButtonColor = Brushes.Gray;
            ActiveCheckerColor = Brushes.Gold;
            ActiveKingCheckerColor = Brushes.Cyan;
            AllowedPositionColor = Brushes.LawnGreen;
            _desk = desk;
        }

        public CellColor GetCellColor()
        {
            return _color;
        }

        public CellPosition GetCellPosition()
        {
            return _position;
        }

        public Button RenderDeskChellAsButton()
        {
            if (Button == null)
            {
                Button = new Button();
                Button.Click += Click;
                Button.Style =
                    (Style) ((MainWindow) Application.Current.MainWindow)?.FindResource("ButtonWithoutHoverEffect");
            }

            Button.Name = "button_" + _position.Get_column() + "_" + _position.Get_row();
            if (_desk.Get_selectedCell() == this)
                if (Checker == null)
                    Button.Background = ActiveButtonColor;
                else
                    Button.Background = !Checker.Is_Quean() ? ActiveCheckerColor : ActiveKingCheckerColor;
            else
                Button.Background = _desk.AllowedPositions.Contains(GetCellPosition())
                    ? AllowedPositionColor
                    : _color.Get_color();

            if (Checker == null)
            {
                Button.Foreground = null;
                Button.Content = "";
            }
            else
            {
                Button.Foreground = Checker.Get_color();
                //            Button.Content = "***" + Environment.NewLine + "***";
                Button.Content = "C:" + _position.Get_column() + Environment.NewLine + "R:" + _position.Get_row();
            }

            if (Tmp)
                Button.Background = Brushes.Black;

            return Button;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            foreach (var cell in _desk.Cells)
            {
                cell.Tmp = false;
            }

            var position = GetCellPosition();
            var startCell = _desk.Cells[position.Get_row() * _desk.Width + position.Get_column()];
            var diagonals = startCell.GetCellDiagonals();
            Diagonal selectedDiagonal = null;
            foreach (var diagonal in diagonals)
            {
                foreach (var diagonalCell in diagonal.Cells)
                {
                    diagonalCell.Tmp = true;
                }

                //                            if (!diagonal.Cells.Contains(cell)) continue;
                //                            selectedDiagonal = diagonal;
                //                            break;
            }
            //
            //            if (Checker == null)
            //            {
            //                if (_desk.AllowedPositions.Contains(_position))
            //                {
            //                    // move to new position
            //                    var cell = _desk.Get_selectedCell();
            //                    Checker = cell.Checker;
            //                    var position = cell.GetCellPosition();
            //                    var startCell = _desk.Cells[position.Get_row() * _desk.Width + position.Get_column()];
            //                    startCell.Checker = null;
            //                    if (!Checker.Is_Quean() &&
            //                        (
            //                            _desk.CurrentWhiteTurn && Checker.Get_isWhite() &&
            //                            _position.Get_row() == _desk.Height - 1 ||
            //                            !_desk.CurrentWhiteTurn && !Checker.Get_isWhite() &&
            //                            _position.Get_row() == 0
            //                        ))
            //                        Checker.SetAsQuean();
            //                    if (_desk.NeedBeat)
            //                    {
            //                        //remove Checkers between startCell and cell
            //                        var diagonals = startCell.GetCellDiagonals();
            //                        Diagonal selectedDiagonal = null;
            //                        foreach (var diagonal in diagonals)
            //                        {
            //                            foreach (var diagonalCell in diagonal.Cells)
            //                            {
            //                                diagonalCell.tmp = true;
            //                            }
            //
            ////                            if (!diagonal.Cells.Contains(cell)) continue;
            ////                            selectedDiagonal = diagonal;
            ////                            break;
            //                        }
            //
            //                        _desk.ReRenderTable();
            //
            //                        if (selectedDiagonal != null)
            //                            foreach (var deskCell in selectedDiagonal.Cells)
            //                            {
            //                                if (deskCell == cell) break;
            //                                _desk.Cells[deskCell.GetCellPosition().Get_row() * _desk.Width + deskCell.GetCellPosition().Get_column()].Checker = null;
            //                            }
            //
            //                        Click(sender, e);
            //                        _desk.CheckIfNeedBeate();
            //                        _desk.ReRenderTable();
            //                    }
            //                    else
            //                    {
            //                        _desk.CurrentWhiteTurn = !_desk.CurrentWhiteTurn;
            //                        _desk.UnselectLastCell();
            //                    }
            //
            //                    _desk.ReRenderTable();
            //                    return;
            //                }
            //
            //                _desk.UnselectLastCell();
            //                _desk.SetSelectedCell(this);
            //            }
            //
            //            if (Checker != null && ((_desk.CurrentWhiteTurn && Checker.Get_isWhite()) ||
            //                                    (!_desk.CurrentWhiteTurn && !Checker.Get_isWhite())))
            //            {
            //                _desk.CheckIfNeedBeate();
            //                _desk.UnselectLastCell();
            //                _desk.SetSelectedCell(this);
            //                if (_desk.NeedBeat)
            //                {
            //                    var allowedPositionsCells = GetBattleCells();
            //                    foreach (var cell in allowedPositionsCells)
            //                    {
            //                        _desk.AllowedPositions.Add(cell._position);
            //                    }
            //                }
            //                else
            //                {
            //                    _desk.ShowAllowedPosition(this);
            //                }
            //            }

            _desk.ReRenderTable();
        }

        public void SetDefaultChecker(Desk desk)
        {
            if (GetCellColor().Get_isWhite()) return;
            var cp = GetCellPosition();
            var match = desk.TopDefaultPositions.FirstOrDefault(
                position => position.Equals(cp)
            );

            if (match != null)
                Checker = new Checker(desk.WhiteOnTop, false);
            else
            {
                match = desk.BottomDefaultPositions.FirstOrDefault(
                    position => position.Equals(cp)
                );

                if (match != null)
                    Checker = new Checker(!desk.WhiteOnTop, false);
            }
        }

        public List<Cell> GetCheckerNeighborsList(bool onlyFront)
        {
            var neighbors = new List<Cell>();
            if (Checker == null) return neighbors;

            var allowRight = true;
            var allowLeft = true;
            int currentIndex = (_position.Get_column() + _position.Get_row() * _desk.Width);
            Cell neighbordCell;
            if (_position.Get_column() == 0)
                allowRight = false;
            if (_position.Get_column() == _desk.Width - 1)
                allowLeft = false;
            if (allowRight)
            {
                var index = currentIndex + (_desk.CurrentWhiteTurn ? (_desk.Width - 1) : (-_desk.Width - 1));
                neighbordCell = _desk.Cells[index];
                neighbors.Add(neighbordCell);
            }

            if (allowLeft)
            {
                var index = currentIndex + (_desk.CurrentWhiteTurn ? (_desk.Width + 1) : (-_desk.Width + 1));
                neighbordCell = _desk.Cells[index];
                neighbors.Add(neighbordCell);
            }

            return neighbors;
        }

        public List<Diagonal> GetCellDiagonals()
        {
            var diagonals = new List<Diagonal>();
            var diagonal = new Diagonal(2);
            var existNextCell = true;
            var currentCell = this;
            var isCurrentCell = true;
            while (existNextCell) // collecting cells to left bottom
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width) + _desk.Width - 1;
                if (currentCellPosition.Get_column() <= 0 || currentCellPosition.Get_row() >= _desk.Height - 1)
                    existNextCell = false;
                if (isCurrentCell)
                    isCurrentCell = false;
                else
                    diagonal.AddCell(currentCell);
                if (existNextCell)
                    currentCell = _desk.Cells[nextPosition];
            }

            diagonals.Add(diagonal);

            diagonal = new Diagonal(1);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) // collecting cells to right top
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width) - _desk.Width + 1;
                if (currentCellPosition.Get_column() >= _desk.Width - 1 || currentCellPosition.Get_row() <= 0)
                    existNextCell = false;
                if (isCurrentCell)
                    isCurrentCell = false;
                else
                    diagonal.AddCell(currentCell);
                if (existNextCell)
                    currentCell = _desk.Cells[nextPosition];
            }

            diagonals.Add(diagonal);


            diagonal = new Diagonal(3);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) // collecting cells to right bottom
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width) + _desk.Width + 1;
                if (currentCellPosition.Get_column() >= _desk.Width - 1 || currentCellPosition.Get_row() >= _desk.Height - 1)
                    existNextCell = false;
                if (isCurrentCell)
                    isCurrentCell = false;
                else
                    diagonal.AddCell(currentCell);
                if (existNextCell)
                    currentCell = _desk.Cells[nextPosition];
            }

            diagonals.Add(diagonal);

            diagonal = new Diagonal(0);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) // collecting cells to left top
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width) - _desk.Width - 1;
                if (currentCellPosition.Get_column() <= 0 || currentCellPosition.Get_row() <= 0)
                    existNextCell = false;
                if (isCurrentCell)
                    isCurrentCell = false;
                else
                    diagonal.AddCell(currentCell);
                if (existNextCell)
                    currentCell = _desk.Cells[nextPosition];
            }

            diagonals.Add(diagonal);
            return diagonals;
        }

        public List<Cell> GetBattleCells()
        {
            var battleCells = new List<Cell>();
            var currentChecker = Checker;
            if (currentChecker == null || currentChecker.Get_isWhite() != _desk.CurrentWhiteTurn) return battleCells;
            var diagonals = GetCellDiagonals();
            foreach (var diagonal in diagonals)
            {
                var enemyCheckersCount = 0;
                foreach (var deskCell in diagonal.Cells)
                {
                    if (deskCell == this)
                        continue;
                    var viewedChecker = deskCell.Checker;
                    if (viewedChecker == null)
                    {
                        if (enemyCheckersCount >= 1)
                        {
                            battleCells.Add(deskCell);
                        }

                        if (!currentChecker.Is_Quean())
                            break;
                    }

                    if (viewedChecker != null && viewedChecker.Get_isWhite() == currentChecker.Get_isWhite())
                        break;
                    ++enemyCheckersCount;
                }
            }

            return battleCells;
        }
    }
}