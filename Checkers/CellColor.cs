using System.Windows.Media;

namespace Checkers
{
    public class CellColor
    {
        private readonly bool _isWhite;

        public CellColor(bool isWhite)
        {
            _isWhite = isWhite;
        }


        public bool Get_isWhite()
        {
            return _isWhite;
        }

        public SolidColorBrush Get_color()
        {
            return _isWhite ? Brushes.Bisque : Brushes.BurlyWood;
        }
    }
}