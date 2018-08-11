using System.Collections.Generic;

namespace Checkers
{
    public class Diagonal
    {
        public List<Cell> Cells = new List<Cell>();
        private readonly int _direction; // 0 - left top; 1 - right top; 2 - left botom; 3 - right botom

        public Diagonal(int direction)
        {
            _direction = direction;
        }

        public void AddCell(Cell cell)
        {
            Cells.Add(cell);
        }

        public int GetDirection()
        {
            return _direction;
        }
    }
}