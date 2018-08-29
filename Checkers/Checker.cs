using System.Windows.Media;

namespace Checkers
{
    public class Checker
    {
        private bool _isQuean;
        private bool _isShotDown;
        private readonly bool _isWhite;

        public Checker(bool isWhite, bool isQuean)
        {
            _isWhite = isWhite;
            _isQuean = isQuean;
        }

        public bool Get_isShotDown()
        {
            return _isShotDown;
        }

        public void ShotDown()
        {
            _isShotDown = true;
        }

        public SolidColorBrush Get_color()
        {
            return _isWhite ? Brushes.DeepPink : Brushes.BlueViolet;
        }

        public string Get_image()
        {
            return Is_Quean() ? (_isWhite ? "images/whiteCrown.png" : "images/blackCrown.png") : (_isWhite ? "images/white.png" : "images/black.png");
        }

        public bool Is_Quean()
        {
            return _isQuean;
        }

        public void SetAsQuean()
        {
            _isQuean = true;
        }

        public bool Get_isWhite()
        {
            return _isWhite;
        }

    }
}