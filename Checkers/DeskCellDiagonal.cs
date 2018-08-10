using System.Collections.Generic;

namespace Checkers
{
    public class DeskCellDiagonal
    {
        public List<DeskCell> Cells = new List<DeskCell>();
        private readonly int _direction; // 0 - left top; 1 - right top; 2 - left botom; 3 - right botom

        public DeskCellDiagonal(int direction)
        {
            _direction = direction;
        }

        public void AddCell(DeskCell cell)
        {
            Cells.Add(cell);
        }

        public int GetDirection()
        {
            return _direction;
        }
    }
}