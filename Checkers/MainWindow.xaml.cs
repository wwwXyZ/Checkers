using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Desk Desk;

        public MainWindow()
        {
            InitializeComponent();
            // Generate checkers grid
            Desk = new Desk();
            GenerateGrid();
        }

        private void btn_new_game_Click(object sender, RoutedEventArgs e)
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
        }

        public void Render_checkers_position(bool viaClear)
        {
            if (viaClear)
            {
                LbWin.Visibility = Visibility.Hidden;
                Ungrd.Children.Clear();
                foreach (var cell in Desk.Cells)
                    Ungrd.Children.Add(cell.RenderDeskChellAsButton());
            }
            else
                for (var i = 0; i < Desk.Cells.Count; i++)
                    CopyControl(Desk.Cells[i].RenderDeskChellAsButton(), Ungrd.Children[i] as Button);
        }

        public void RenderBattlefield(bool viaClear)
        {
            Render_checkers_position(viaClear);
            var whiteCount = Desk.Get_whiteCount();
            var blackCount = Desk.Get_blackCount();
            LbWhiteCount.Content = whiteCount;
            LbBlackCount.Content = blackCount;
            if (whiteCount <= 0)
                EngGame(1);
            if (blackCount <= 0)
                EngGame(0);
            LbCurrentTurn.Content = Desk.CurrentWhiteTurn ? "White" : "Black";
        }

        public void EngGame(int condition) // 0 - BLACK WIN; 1 - WHITE WIN; -1 - DRAW
        {
            var content = "";
            switch (condition)
            {
                case 0:
                    content = "BLACK IS WIN!";
                    break;
                case 1:
                    content = "WHITE IS WIN!";
                    break;
                default:
                    content = "DRAW :(";
                    break;
            }

            LbWin.Content = content;
            LbWin.Visibility = Visibility.Visible;
        }

        protected virtual void CopyControl(Control sourceControl, Control targetControl)
        {
            // make sure these are the same
            if (sourceControl.GetType() != targetControl.GetType())
            {
                throw new Exception("Incorrect control types");
            }

            foreach (PropertyInfo sourceProperty in sourceControl.GetType().GetProperties())
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
    }
}