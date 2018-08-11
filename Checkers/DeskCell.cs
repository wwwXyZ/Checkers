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
    public partial class DeskCell
    {
        private readonly DeskCellColor _color;
        private readonly DeskCellPosition _position;
        public DeskCellChecker Checker;
        public SolidColorBrush ActiveButtonColor;
        public SolidColorBrush ActiveCheckerColor;
        public SolidColorBrush ActiveKingCheckerColor;
        public SolidColorBrush AllowedPositionColor;
        public Button Button;
        private readonly CheckersDesk _parent;

        public DeskCell(DeskCellPosition position, DeskCellColor color, DeskCellChecker checker, CheckersDesk parent)
        {
            _position = position;
            _color = color;
            Checker = checker;
            ActiveButtonColor = Brushes.Gray;
            ActiveCheckerColor = Brushes.Gold;
            ActiveKingCheckerColor = Brushes.Cyan;
            AllowedPositionColor = Brushes.LawnGreen;
            _parent = parent;
        }

        public DeskCellColor GetCellColor()
        {
            return _color;
        }

        public DeskCellPosition GetCellPosition()
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

            Button.Name = "button_" + _position.get_column() + "_" + _position.get_row();
            if (_parent.Get_selectedCell() == this)
                if (Checker == null)
                    Button.Background = ActiveButtonColor;
                else
                    Button.Background = !Checker.Is_king() ? ActiveCheckerColor : ActiveKingCheckerColor;
            else
                Button.Background = _parent.AllowedPositions.Contains(GetCellPosition())
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
                Button.Content = "C:" + _position.get_column() + Environment.NewLine + "R:" + _position.get_row();
            }

            return Button;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            if (Checker == null)
            {
                if (_parent.AllowedPositions.Contains(_position))
                {
                    // move to new position
                    var cell = _parent.Get_selectedCell();
                    Checker = cell.Checker;
                    var position = cell.GetCellPosition();
                    var startCell = _parent.Cells[position.get_row() * _parent.Width + position.get_column()];
                    startCell.Checker = null;
                    if (!Checker.Is_king() &&
                        (
                            _parent.CurrentWhiteTurn && Checker.Is_white() &&
                            _position.get_row() == _parent.Height - 1 ||
                            !_parent.CurrentWhiteTurn && !Checker.Is_white() &&
                            _position.get_row() == 0
                        ))
                        Checker.SetAsKing();
                    if (_parent.NeedBeat)
                    {
                        //remove Checkers between startCell and cell
                        var diagonals = startCell.GetCellDiagonals();
                        DeskCellDiagonal selectedDiagonal = null;
                        foreach (var diagonal in diagonals)
                        {
                            if (!diagonal.Cells.Contains(cell)) continue;
                            selectedDiagonal = diagonal;
                            break;
                        }

                        if (selectedDiagonal != null)
                            foreach (var deskCell in selectedDiagonal.Cells)
                            {
                                if(deskCell == cell) break;
                                _parent.Cells[deskCell.GetCellPosition().get_row() * _parent.Width + deskCell.GetCellPosition().get_column()].Checker = null;
                            }

                        Click(sender, e);
                        _parent.CheckIfNeedBeate();
                        _parent.ReRenderTable();
                    }
                    else
                    {
                        _parent.CurrentWhiteTurn = !_parent.CurrentWhiteTurn;
                        _parent.UnselectLastCell();
                    }

                    _parent.ReRenderTable();
                    return;
                }

                _parent.UnselectLastCell();
                _parent.SetSelectedCell(this);
            }

            if (Checker != null && ((_parent.CurrentWhiteTurn && Checker.Is_white()) ||
                                    (!_parent.CurrentWhiteTurn && !Checker.Is_white())))
            {
                _parent.CheckIfNeedBeate();
                _parent.UnselectLastCell();
                _parent.SetSelectedCell(this);
                if (_parent.NeedBeat)
                {
                    var allowedPositionsCells = GetBattleCells();
                    foreach (var cell in allowedPositionsCells)
                    {
                        _parent.AllowedPositions.Add(cell._position);
                    }
                }
                else
                {
                    _parent.ShowAllowedPosition(this);
                }
            }

            _parent.ReRenderTable();
        }

        public void SetDefaultChecker(CheckersDesk desk)
        {
            if (GetCellColor().Get_isWhite()) return;
            var cp = GetCellPosition();
            var match = desk.TopDefaultPositions.FirstOrDefault(
                position => position.Equals(cp)
            );

            if (match != null)
                Checker = new DeskCellChecker(desk.WhiteOnTop, false);
            else
            {
                match = desk.BottomDefaultPositions.FirstOrDefault(
                    position => position.Equals(cp)
                );

                if (match != null)
                    Checker = new DeskCellChecker(!desk.WhiteOnTop, false);
            }
        }

        public List<DeskCell> GetCheckerNeighborsList(bool onlyFront)
        {
            var neighbors = new List<DeskCell>();
            if (Checker == null) return neighbors;

            var allowRight = true;
            var allowLeft = true;
            int currentIndex = (_position.get_column() + _position.get_row() * _parent.Width);
            DeskCell neighbordCell;
            if (_position.get_column() == 0)
                allowRight = false;
            if (_position.get_column() == _parent.Width - 1)
                allowLeft = false;
            if (allowRight)
            {
                var index = currentIndex + (_parent.CurrentWhiteTurn ? (_parent.Width - 1) : (-_parent.Width - 1));
                neighbordCell = _parent.Cells[index];
                neighbors.Add(neighbordCell);
            }

            if (allowLeft)
            {
                var index = currentIndex + (_parent.CurrentWhiteTurn ? (_parent.Width + 1) : (-_parent.Width + 1));
                neighbordCell = _parent.Cells[index];
                neighbors.Add(neighbordCell);
            }

            return neighbors;
        }

        public List<DeskCellDiagonal> GetCellDiagonals()
        {
            var diagonals = new List<DeskCellDiagonal>();
            var diagonal = new DeskCellDiagonal(2);
            var existNextCell = true;
            var currentCell = this;
            var isCurrentCell = true;
            while (existNextCell) // collecting cells to left bottom
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.get_column() + currentCellPosition.get_row() * _parent.Width) + _parent.Width - 1;
                if (currentCellPosition.get_column() == 0 || nextPosition > _parent.Width * _parent.Height - 1)
                    existNextCell = false;
                else
                {
                    if (isCurrentCell)
                        isCurrentCell = false;
                    else
                        diagonal.AddCell(currentCell);
                    currentCell = _parent.Cells[nextPosition];
                }
            }

            diagonals.Add(diagonal);

            diagonal = new DeskCellDiagonal(1);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) //collecting cells to right top
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.get_column() + currentCellPosition.get_row() * _parent.Width) - _parent.Width + 1;
                if (currentCellPosition.get_column() == _parent.Width - 1 || nextPosition < 0)
                    existNextCell = false;
                else
                {
                    if (isCurrentCell)
                        isCurrentCell = false;
                    else
                        diagonal.AddCell(currentCell);
                    currentCell = _parent.Cells[nextPosition];
                }
            }

            diagonals.Add(diagonal);


            diagonal = new DeskCellDiagonal(3);
            existNextCell = true;
            currentCell = this;
            isCurrentCell = true;
            while (existNextCell) // collecting cells right bottom
            {
                var currentCellPosition = currentCell.GetCellPosition();
                if (currentCellPosition.get_column() == _parent.Width - 1 || currentCellPosition.get_row() == _parent.Width - 1)
                    existNextCell = false;
                else
                {
                    if (isCurrentCell)
                        isCurrentCell = false;
                    else
                        diagonal.AddCell(currentCell);
                    currentCell = _parent.Cells[(currentCellPosition.get_column() + currentCellPosition.get_row() * _parent.Width) + _parent.Width + 1];
                }
            }

            diagonals.Add(diagonal);

            diagonal = new DeskCellDiagonal(0);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) //collecting cells to left top
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = (currentCellPosition.get_column() + currentCellPosition.get_row() * _parent.Width) - _parent.Width - 1;
                if (currentCellPosition.get_column() == 0 || nextPosition < 0)
                    existNextCell = false;
                else
                {
                    if (isCurrentCell)
                        isCurrentCell = false;
                    else
                        diagonal.AddCell(currentCell);
                    currentCell = _parent.Cells[nextPosition];
                }
            }

            diagonals.Add(diagonal);
            return diagonals;
        }

        public List<DeskCell> GetBattleCells()
        {
            var battleCells = new List<DeskCell>();
            var currentChecker = Checker;
            if (currentChecker == null || currentChecker.Is_white() != _parent.CurrentWhiteTurn) return battleCells;
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

                        if (!currentChecker.Is_king())
                            break;
                    }

                    if (viewedChecker != null && viewedChecker.Is_white() == currentChecker.Is_white())
                        break;
                    ++enemyCheckersCount;
                }
            }

            return battleCells;
        }
    }
}