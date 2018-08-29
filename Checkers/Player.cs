using System;

namespace Checkers
{
    public class Player
    {
        public Player(string rawPlayer)
        {
            var intPlayer = int.Parse(rawPlayer);
            switch (intPlayer)
            {
                case 0:
                    IsWhite = false;
                    IsHuman = true;
                    break;
                case 1:
                    IsWhite = true;
                    IsHuman = true;
                    break;
                case 2:
                    IsWhite = true;
                    IsHuman = false;
                    break;
                case 3:
                    IsWhite = false;
                    IsHuman = false;
                    break;
                default:
                    throw new Exception("Empty Player constructor!");
            }
        }

        public Player(bool isHuman, bool isWhite)
        {
            IsHuman = isHuman;
            IsWhite = isWhite;
        }

        public bool IsHuman { get; }

        public bool IsWhite { get; }

        /*
        * 0 = black human
        * 1 = white human
        * 2 = white bot
        * 3 = black bot
        */
        public string ReturnPlayerAsRawText()
        {
            return (IsHuman ? (IsWhite ? 1 : 0) : (IsWhite ? 2 : 3)).ToString();
        }

    }
}