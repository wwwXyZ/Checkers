using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public Cell(CellPosition position, string rawCell, Desk desk)
        {
            Checker checker;
            var rawChrcker = int.Parse(rawCell.Substring(1, 1));
            switch (rawChrcker)
            {
                case 1:
                    checker = new Checker(false, false);
                    break;
                case 2:
                    checker = new Checker(false, true);
                    break;
                case 3:
                    checker = new Checker(true, false);
                    break;
                case 4:
                    checker = new Checker(true, true);
                    break;
                default:
                    checker = null;
                    break;
            }

            _position = position;
            _color = new CellColor(rawCell[0] == '1');
            Checker = checker;
            ActiveButtonColor = Brushes.Gray;
            ActiveCheckerColor = Brushes.Gold;
            ActiveKingCheckerColor = Brushes.Cyan;
            AllowedPositionColor = Brushes.LawnGreen;
            _desk = desk;
        }

        public CellColor GetCellColor() => _color;

        public CellPosition GetCellPosition() => _position;

        public StackPanel ConstructStackPanel(string imageSource)
        {
            var stackPnl = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5)
            };
            stackPnl.Children.Add(new Image {Source = new BitmapImage(new Uri(imageSource, UriKind.RelativeOrAbsolute))});
            return stackPnl;
        }

        public Button RenderDeskChellAsButton()
        {
            if (Button == null)
            {
                Button = new Button();
                Button.Click += Click;
                Button.Style = (Style) ((MainWindow) Application.Current.MainWindow)?.FindResource("ButtonWithoutHoverEffect");
            }

            Button.Name = "button_" + _position.Get_column() + "_" + _position.Get_row();
            if (_desk.Get_selectedCell() == this)
                if (Checker == null)
                    Button.Background = ActiveButtonColor;
                else
                    Button.Background = !Checker.Is_Quean() ? ActiveCheckerColor : ActiveKingCheckerColor;
            else
                Button.Background = _desk.AllowedPositions.Contains(GetCellPosition()) ? AllowedPositionColor : _color.Get_color();

            if (Checker == null)
            {
                Button.Foreground = null;
                Button.Content = "";
            }
            else
            {
//                Button.Foreground = Checker.Get_image();
                //            Button.Content = "***" + Environment.NewLine + "***";

                Button.Content = ConstructStackPanel(Checker.Get_image());
                if (Checker.Get_isShotDown())
                    Button.Background = Brushes.Green;
            }

            if (_desk.BattleCheckersPositions.Contains(_position))
                Button.Background = Brushes.Red;
            return Button;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            if (_desk.CurrentPlayerIsHuman())
                Click(true);
        }

        public void Click(bool allowRender)
        {
            if (_desk.Get_finishedGame())
                return;
            if (Checker == null)
            {
//                _desk.Set_ShotDownCeckerCell(null);
                if (_desk.AllowedPositions.Contains(_position))
                {
                    // move to new position
                    var cell = _desk.Get_selectedCell();
                    Checker = cell.Checker;
                    var position = cell.GetCellPosition();
                    var startCell = _desk.GetCell(position);
                    startCell.Checker = null;
                    if (!Checker.Is_Quean() &&
                        (
                            _desk.CurrentWhiteTurn && Checker.Get_isWhite() &&
                            _position.Get_row() == _desk.Height - 1 ||
                            !_desk.CurrentWhiteTurn && !Checker.Get_isWhite() &&
                            _position.Get_row() == 0
                        ))
                        Checker.SetAsQuean();
                    if (_desk.NeedBeat)
                    {
                        //remove Checkers between startCell and cell
                        var diagonals = startCell.GetCellDiagonals();
                        Diagonal selectedDiagonal = null;
                        foreach (var diagonal in diagonals)
                        {
                            if (!diagonal.Cells.Contains(this)) continue;
                            selectedDiagonal = diagonal;
                            break;
                        }

                        if (selectedDiagonal != null)
                            foreach (var deskCell in selectedDiagonal.Cells)
                            {
                                var selectedChecker = _desk.GetCell(deskCell.GetCellPosition()).Checker;

                                if (selectedChecker == null) continue;
                                if (deskCell == this) break;
                                if (selectedChecker.Get_isWhite())
                                    _desk.Set_whiteCount(_desk.Get_whiteCount() - 1);
                                else
                                    _desk.Set_blackCount(_desk.Get_blackCount() - 1);
                                if (deskCell.Checker == null) continue;
                                selectedChecker.ShotDown();
                                _desk.Set_ShotDownCeckerCell(deskCell);
                                break;
                            }

                        Click(false);
                        _desk.CheckIfNeedBeate(this);
                    }

                    if (!_desk.NeedBeat)
                    {
                        _desk.BattleCheckersPositions.Clear();
                        _desk.CurrentWhiteTurn = !_desk.CurrentWhiteTurn;
                        _desk.EndTurn(); //todo: need good working
                        if (!_desk.Get_isBotSimulation() && _desk.Get_finishedGame())
                            return;
                        _desk.UnselectLastCell();
                        _desk.CheckIfNeedBeate();
                        if (allowRender)
                            _desk.ReRenderTable(Desk.BotStepTimeout);
                        _desk.BotTurn();
                        if (allowRender)
                            _desk.ReRenderTable(Desk.BotStepTimeout);
                    }

                    if (allowRender)
                        _desk.ReRenderTable();

                    return;
                }

                _desk.UnselectLastCell();
                _desk.SetSelectedCell(this);
            }

            if (Checker != null)
            {
                if (_desk.Get_allowCheats() && !_desk.Get_isBotSimulation() && _desk.CurrentPlayerIsHuman() && Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt)) //Cheats
                {
                    if (Keyboard.IsKeyDown(Key.Q))
                        Checker.SetAsQuean();
                    if (Keyboard.IsKeyDown(Key.D))
                    {
                        if (!Checker.Get_isShotDown() && Checker.Get_isWhite())
                            _desk.Set_whiteCount(_desk.Get_whiteCount() - 1);
                        else
                            _desk.Set_blackCount(_desk.Get_blackCount() - 1);
                        Checker = null;
                        _desk.AllowedPositions.Clear();
                        _desk.BattleCheckersPositions.Clear();
                        if (_desk.Get_selectedCell() != null && _desk.Get_selectedCell().Checker != null)
                        {
                            _desk.Get_selectedCell().GetBattleCells();
                            if (_desk.BattleCheckersPositions.Count <= 0)
                                _desk.Get_selectedCell().GetAllowedPositions();
                            else
                            {
                                _desk.NeedBeat = true;
                                foreach (var cell in _desk.Get_selectedCell().GetBattleCells())
                                    _desk.AllowedPositions.Add(cell._position);
                            }
                        }

                        if (allowRender)
                            _desk.ReRenderTable();
                        return;
                    }
                }

                if (_desk.CurrentWhiteTurn && Checker.Get_isWhite() ||
                    !_desk.CurrentWhiteTurn && !Checker.Get_isWhite())
                {
                    _desk.UnselectLastCell();
                    _desk.SetSelectedCell(this);
                    _desk.CheckIfNeedBeate();
                    if (_desk.NeedBeat)
                    {
                        var allowedPositionsCells = GetBattleCells();
                        foreach (var cell in allowedPositionsCells) _desk.AllowedPositions.Add(cell._position);
                    }
                    else
                    {
                        _desk.ShowAllowedPosition(this);
                    }

                    _desk.BotTurn();
                    if (allowRender)
                        _desk.ReRenderTable(Desk.BotStepTimeout);
                }
            }

            if (allowRender)
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
            {
                Checker = new Checker(true, false);
            }
            else
            {
                match = desk.BottomDefaultPositions.FirstOrDefault(
                    position => position.Equals(cp)
                );

                if (match != null)
                    Checker = new Checker(false, false);
            }
        }

        public List<Cell> GetCheckerNeighborsList(bool onlyFront)
        {
            var neighbors = new List<Cell>();
            if (Checker == null) return neighbors;

            var allowRight = true;
            var allowLeft = true;
            var currentIndex = _position.Get_column() + _position.Get_row() * _desk.Width;
            Cell neighbordCell;
            var itsLastCheckerLine = _desk.CurrentWhiteTurn ? _position.Get_row() >= _desk.Width - 1 : _position.Get_row() <= 0;
            if (_position.Get_column() == 0 || itsLastCheckerLine)
                allowRight = false;
            if (_position.Get_column() == _desk.Width - 1 || itsLastCheckerLine)
                allowLeft = false;
            if (allowRight)
            {
                var index = currentIndex + (_desk.CurrentWhiteTurn ? _desk.Width - 1 : -_desk.Width - 1);
                neighbordCell = _desk.Cells[index];
                neighbors.Add(neighbordCell);
            }

            // ReSharper disable once InvertIf
            if (allowLeft)
            {
                var index = currentIndex + (_desk.CurrentWhiteTurn ? _desk.Width + 1 : -_desk.Width + 1);
                neighbordCell = _desk.Cells[index];
                neighbors.Add(neighbordCell);
            }

            return neighbors;
        }

        public List<Diagonal> GetCellDiagonals()
        {
            var diagonals = new List<Diagonal>();
            var diagonal = new Diagonal(0);
            var existNextCell = true;
            var currentCell = this;
            var isCurrentCell = true;
            while (existNextCell) // collecting cells to left top
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width - _desk.Width - 1;
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

            diagonal = new Diagonal(1);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) // collecting cells to right top
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width - _desk.Width + 1;
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

            diagonal = new Diagonal(2);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) // collecting cells to left bottom
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width + _desk.Width - 1;
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

            diagonal = new Diagonal(3);
            currentCell = this;
            isCurrentCell = true;
            existNextCell = true;
            while (existNextCell) // collecting cells to right bottom
            {
                var currentCellPosition = currentCell.GetCellPosition();
                var nextPosition = currentCellPosition.Get_column() + currentCellPosition.Get_row() * _desk.Width + _desk.Width + 1;
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


            return diagonals;
        }

        public List<Cell> GetBattleCells()
        {
            var battleCells = new List<Cell>();
            var currentChecker = Checker;
            if (currentChecker == null || currentChecker.Get_isShotDown() || currentChecker.Get_isWhite() != _desk.CurrentWhiteTurn) return battleCells;
            var diagonals = GetCellDiagonals();
            foreach (var diagonal in diagonals)
            {
                var enemyCheckersCount = 0;
                foreach (var deskCell in diagonal.Cells)
                {
                    var viewedChecker = deskCell.Checker;
                    if (viewedChecker == null)
                    {
                        if (enemyCheckersCount == 1)
                        {
                            if (!battleCells.Contains(deskCell))
                                battleCells.Add(deskCell);
                            if (!_desk.BattleCheckersPositions.Contains(_position))
                                _desk.BattleCheckersPositions.Add(_position);
                        }

                        if (!currentChecker.Is_Quean())
                            break;
                    }

                    if (enemyCheckersCount > 1 || viewedChecker != null && viewedChecker.Get_isWhite() == currentChecker.Get_isWhite())
                        break;
                    if (viewedChecker != null && viewedChecker.Get_isShotDown()) break; //not shure

                    if (viewedChecker != null && !viewedChecker.Get_isShotDown())
                        ++enemyCheckersCount;
                }
            }

            return battleCells;
        }

        public List<CellPosition> GetAllowedPositions()
        {
            var positions = new List<CellPosition>();
            var checker = Checker;
            if (!checker.Is_Quean())
            {
                var neighbors = GetCheckerNeighborsList(true);
                foreach (var neighbordCell in neighbors)
                    if (neighbordCell.Checker == null)
                        positions.Add(neighbordCell.GetCellPosition());
            }
            else
            {
                var diagonals = GetCellDiagonals();
                foreach (var diagonal in diagonals)
                foreach (var diagonalCell in diagonal.Cells)
                {
                    if (diagonalCell.Checker != null) break;
                    positions.Add(diagonalCell.GetCellPosition());
                }
            }

            return positions;
        }

        /*
         * First number = CellColor (Black 0; White 1)
         * Second number = Cecker (null 0; Black 1; Black Quean 2; White 3; White Quean 4)
         */
        public string ReturnCellAsRawText()
        {
            var number = _color.Get_isWhite() ? "1" : "0";
            number += Checker == null ? "0" : (Checker.Get_isWhite() ? (Checker.Is_Quean() ? "4" : "3") : (Checker.Is_Quean() ? "2" : "1"));
            return number;
        }
    }
}