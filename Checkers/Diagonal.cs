using System.Collections.Generic;

namespace Checkers
{
    public class Diagonal
    {
        public List<Cell> Cells { get; } = new List<Cell>();

        public int Direction { get; }

        public Diagonal(int direction)
        {
            Direction = direction;
        }
    }
}