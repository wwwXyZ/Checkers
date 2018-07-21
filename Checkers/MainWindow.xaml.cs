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
        public CheckersDesk Desk;

        public MainWindow()
        {
            InitializeComponent();
            // Generate checkers grid
            Desk = new CheckersDesk();
            generate_grid();
        }

        private void btn_new_game_Click(object sender, RoutedEventArgs e)
        {
        }

        public void generate_grid()
        {
            var width = Desk.Width;
            var height = Desk.Height;
            Ungrd.Columns = width;
            Ungrd.Rows = height;

            Desk.Clear_cells();
            Desk.Generate(true);
            Render_checkers_position(true);
        }

        public void Render_checkers_position(bool viaClear)
        {
            if (viaClear)
            {
                Ungrd.Children.Clear();
                foreach (var cell in Desk.Cells)
                {
                    var button = cell.RenderDeskChellAsButton();
                    Ungrd.Children.Add(button);
                }
            }
            else
            {
                for (var i = 0; i < Desk.Cells.Count; i++)
                {
                    var button = Ungrd.Children[i] as Button;

                    var btn = Desk.Cells[i].RenderDeskChellAsButton();
                    copyControl(btn, button);
                }
            }
        }

        private void copyControl(Control sourceControl, Control targetControl)
        {
            // make sure these are the same
            if (sourceControl.GetType() != targetControl.GetType())
            {
                throw new Exception("Incorrect control types");
            }

            foreach (PropertyInfo sourceProperty in sourceControl.GetType().GetProperties())
            {
                object newValue = sourceProperty.GetValue(sourceControl, null);

                MethodInfo mi = sourceProperty.GetSetMethod(true);
                if (mi != null)
                {
                    sourceProperty.SetValue(targetControl, newValue, null);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Width / Height != 1)
            {
                this.Width = this.Height;
            }
        }
    }
}