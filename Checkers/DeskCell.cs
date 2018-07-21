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
                    : _color.get_color();

            if (Checker == null)
            {
                Button.Foreground = null;
                Button.Content = "";
            }
            else
            {
                Button.Foreground = Checker.get_color();
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
                    _parent.Cells[position.get_row() * _parent.Width + position.get_column()].Checker = null;
                    if (!Checker.Is_king() &&
                        (
                            _parent.CurrentWhiteTurn && Checker.Is_white() &&
                            _position.get_row() == _parent.Height - 1 ||
                            !_parent.CurrentWhiteTurn && !Checker.Is_white() &&
                            _position.get_row() == 0
                        ))
                        Checker.SetAsKing();
                    _parent.CurrentWhiteTurn = !_parent.CurrentWhiteTurn;
                    _parent.UnselectLastCell();
                    _parent.ReRenderTable();
                    return;
                }

                _parent.UnselectLastCell();
                _parent.SetSelectedCell(this);
            }

            if (Checker != null && ((_parent.CurrentWhiteTurn && Checker.Is_white()) ||
                                    (!_parent.CurrentWhiteTurn && !Checker.Is_white())))
            {
                _parent.UnselectLastCell();
                _parent.SetSelectedCell(this);
                _parent.ShowAllowedPosition(this);
            }

            _parent.ReRenderTable();
        }

        public void SetDefaultChecker(CheckersDesk desk)
        {
            if (GetCellColor().get_isWhite()) return;
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
    }
}