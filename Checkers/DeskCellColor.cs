using System.Windows.Media;

namespace Checkers
{
    public partial class DeskCell
    {
        public class DeskCellColor
        {
            private readonly bool _isWhite;


            public bool Get_isWhite()
            {
                return _isWhite;
            }

            public DeskCellColor(bool isWhite)
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