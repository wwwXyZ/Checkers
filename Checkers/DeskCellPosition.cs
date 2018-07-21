using System;

namespace Checkers
{
    public partial class DeskCell
    {
        public class DeskCellPosition
        {
            private readonly int _column;
            private readonly int _row;


            public int get_row()
            {
                return _row;
            }

            public int get_column()
            {
                return _column;
            }

            public int[] Get_position()
            {
                int[] position = {_column, _row};
                return position;
            }

            public DeskCellPosition(int column, int row)
            {
                _column = column;
                _row = row;
            }

            public new virtual bool Equals(object obj)
            {
                return obj is DeskCellPosition position && (position._column == _column && position._row == _row);
            }
        }
    }
}