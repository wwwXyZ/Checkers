using System.Windows.Media;

namespace Checkers
{
    public class Checker
    {
        public Checker(bool isWhite, bool isQuean)
        {
            IsWhite = isWhite;
            IsQuean = isQuean;
        }

        public bool IsShotDown { get; private set; }

        public void ShotDown()
        {
            IsShotDown = true;
        }

        public SolidColorBrush Get_color()
        {
            return IsWhite ? Brushes.DeepPink : Brushes.BlueViolet;
        }

        public string Get_image()
        {
            return IsQuean ? (IsWhite ? "images/whiteCrown.png" : "images/blackCrown.png") : (IsWhite ? "images/white.png" : "images/black.png");
        }

        public bool IsQuean { get; private set; }

        public void SetAsQuean()
        {
            IsQuean = true;
        }

        public bool IsWhite { get; }
    }
}