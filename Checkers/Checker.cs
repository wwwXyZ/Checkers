using System;
using System.Windows.Media;

namespace Checkers
{
    public class Checker
    {
        private bool _isQuean;
        private readonly bool _isWhite;

        public Checker(bool isWhite, bool isQuean)
        {
            _isWhite = isWhite;
            _isQuean = isQuean;
        }

        public SolidColorBrush Get_color()
        {
            return _isWhite ? Brushes.DeepPink : Brushes.BlueViolet;
        }

        public String Get_image()
        {
            return _isWhite ? "images/putin.png" : "images/trump.png";
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