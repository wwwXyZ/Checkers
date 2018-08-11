using System.Windows.Media;

namespace Checkers
{
    public partial class Cell
    {
        public class CellColor
        {
            private readonly bool _isWhite;


            public bool Get_isWhite()
            {
                return _isWhite;
            }

            public CellColor(bool isWhite)
            {
                _isWhite = isWhite;
            }

            public SolidColorBrush Get_color()
            {
                return _isWhite ? Brushes.Bisque : Brushes.BurlyWood;
            }
        }
    }
}