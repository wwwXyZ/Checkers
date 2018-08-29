using System;

namespace Checkers
{
    public class Player
    {
        private readonly bool _isHuman;
        private readonly bool _isWhite;

        public Player(string rawPlayer)
        {
            var intPlayer = int.Parse(rawPlayer);
            switch (intPlayer)
            {
                case 0:
                    _isWhite = false;
                    _isHuman = true;
                    break;
                case 1:
                    _isWhite = true;
                    _isHuman = true;
                    break;
                case 2:
                    _isWhite = true;
                    _isHuman = false;
                    break;
                case 3:
                    _isWhite = false;
                    _isHuman = false;
                    break;
                default:
                    throw new Exception("Empty Player constructor!");
            }
        }

        public Player(bool isHuman, bool isWhite)
        {
            _isHuman = isHuman;
            _isWhite = isWhite;
        }

        public bool Get_isHuman()
        {
            return _isHuman;
        }

        public bool Get_isWhite()
        {
            return _isWhite;
        }

        /*
        * 0 = black human
        * 1 = white human
        * 2 = white bot
        * 3 = black bot
        */
        public string ReturnPlayerAsRawText()
        {
            return (_isHuman ? (_isWhite ? 1 : 0) : (_isWhite ? 2 : 3)).ToString();
        }

    }
}