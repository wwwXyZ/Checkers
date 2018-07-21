using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Checkers
{
    public partial class DeskCell : CheckersDesk
    {
        private readonly DeskCellColor _color;
        private readonly DeskCellPosition _position;
        public DeskCellChecker Checker;
        public SolidColorBrush ActiveButtonColor;
        public SolidColorBrush ActiveCheckerColor;
        public SolidColorBrush ActiveKingCheckerColor;
        public Button Button;

        public DeskCell(DeskCellPosition position, DeskCellColor color, DeskCellChecker checker)
        {
            _position = position;
            _color = color;
            Checker = checker;
            ActiveButtonColor = Brushes.Gray;
            ActiveCheckerColor = Brushes.Gold;
            ActiveKingCheckerColor = Brushes.Cyan;
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
                Button.Click += ShowAllowPosition;
            }

            Button.Name = "button_" + _position.get_column() + "_" + _position.get_row();
            Button.Background = _color.get_color();
            if (Checker == null) return Button;
            Button.Foreground = Checker.get_color();
            Button.Content = "***" + Environment.NewLine + "***" + Environment.NewLine + "***";
            return Button;
        }

        private void ShowAllowPosition(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            UnselectLastCell();
            if (Checker == null)
                button.Background = ActiveButtonColor;
            else if (CurrentWhiteTurn && Checker.Is_white())
                button.Background = !Checker.Is_king() ? ActiveCheckerColor : ActiveKingCheckerColor;
            else
            {
                if (CurrentWhiteTurn || Checker.Is_white()) return;
                button.Background = !Checker.Is_king() ? ActiveCheckerColor : ActiveKingCheckerColor;
            }
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