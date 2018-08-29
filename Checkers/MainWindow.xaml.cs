using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Checkers.AI;

namespace Checkers
{
    public partial class MainWindow : Window
    {
        public Desk Desk;

        public MainWindow()
        {
            InitializeComponent();
            // Generate checkers grid
            Desk = new Desk(8, 8, 0, false, false, false);
            GenerateGrid();
        }

        private void BtnNewGameClick(object sender, RoutedEventArgs e)
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            var width = Desk.Width;
            var height = Desk.Height;
            Ungrd.Columns = width;
            Ungrd.Rows = height;
            Desk.Clear_cells();
            Desk.Generate(true);
            RenderBattlefield(true);
            UpdateRotation();
            if (!Desk.FirstPlayer.Get_isHuman() && Desk.CurrentWhiteTurn || !Desk.SecondPlayer.Get_isHuman() && !Desk.CurrentWhiteTurn)
                Desk.BotTurn();
        }

        public void Render_checkers_position(bool viaClear)
        {
            if (viaClear)
            {
                Desk.AllowedPositions.Clear();
                Desk.BattleCheckersPositions.Clear();
                LbWin.Visibility = Visibility.Hidden;
                Ungrd.Children.Clear();
                foreach (var cell in Desk.Cells)
                    Ungrd.Children.Add(cell.RenderDeskChellAsButton());
            }
            else
                for (var i = 0; i < Desk.Cells.Count; i++)
                    CopyControl(Desk.Cells[i].RenderDeskChellAsButton(), Ungrd.Children[i] as Button);
        }

        public void UpdateRotation()
        {
            var angle = 90 * Desk.Rotation;
            var rotateTransform = new RotateTransform {Angle = angle};
            Ungrd.RenderTransform = rotateTransform;
        }

        public void RenderBattlefield(bool viaClear)
        {
            Render_checkers_position(viaClear);
            var whiteCount = Desk.Get_whiteCount();
            var blackCount = Desk.Get_blackCount();
            LbWhiteCount.Content = whiteCount;
            LbBlackCount.Content = blackCount;
            if (whiteCount <= 0)
                EngGame(-2);
            if (blackCount <= 0)
                EngGame(2);
            LbCurrentTurn.Content = Desk.CurrentWhiteTurn ? "White" : "Black";
        }

        /*
         *  -2 - BLACK WINS;
         *  -1 - BLACK DRAW;
         *   0 - FULL DRAW;
         *   1 - WHITE DRAW;
         *   2 - WHITE WINS
         */
        public void EngGame(int condition)
        {
            var content = "";
            switch (condition)
            {
                case -2:
                    content = "BLACK WINS!";
                    break;
                case -1:
                    content = "BLACK DRAW";
                    break;
                case 0:
                    content = "FULL DRAW";
                    break;
                case 1:
                    content = "WHITE DRAW";
                    break;
                case 2:
                    content = "WHITE WINS!";
                    break;
                default:
                    break;
            }

            LbWin.Content = content;
            LbWin.Visibility = Visibility.Visible;
            BtnNewGameClick(null, null);
        }

        protected virtual void CopyControl(Control sourceControl, Control targetControl)
        {
            // make sure these are the same
            if (sourceControl.GetType() != targetControl.GetType())
            {
                throw new Exception("Incorrect control types");
            }

            foreach (var sourceProperty in sourceControl.GetType().GetProperties())
            {
                var newValue = sourceProperty.GetValue(sourceControl, null);

                var mi = sourceProperty.GetSetMethod(true);
                if (mi != null)
                {
                    sourceProperty.SetValue(targetControl, newValue, null);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Width / Height != 1)
            {
                Width = Height;
            }
        }

        private void AllowCheats_Click(object sender, RoutedEventArgs e)
        {
            Desk.Toggle_allowCheats();
        }

        private void AutoRotate_Click(object sender, RoutedEventArgs e)
        {
            Desk.Toggle_autoRotate();
        }

        private void LeftRotateBtn_Click(object sender, RoutedEventArgs e)
        {
            Desk.Rotation -= 1;
            UpdateRotation();
        }

        private void RightRotateBtn_Click(object sender, RoutedEventArgs e)
        {
            Desk.Rotation += 1;
            UpdateRotation();
        }
    }
}