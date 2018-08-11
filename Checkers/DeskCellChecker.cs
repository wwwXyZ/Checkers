using System.Windows.Media;

namespace Checkers
{
    public class DeskCellChecker
    {
        private bool _isKing;
        private readonly bool is_white;

        public DeskCellChecker(bool isWhite, bool isKing)
        {
            is_white = isWhite;
            _isKing = isKing;
        }

        public SolidColorBrush Get_color()
        {
            return is_white ? Brushes.DeepPink : Brushes.BlueViolet;
        }

        public bool Is_king()
        {
            return _isKing;
        }

        public void SetAsKing()
        {
            _isKing = true;
        }

        public bool Is_white()
        {
            return is_white;
        }

    }
}