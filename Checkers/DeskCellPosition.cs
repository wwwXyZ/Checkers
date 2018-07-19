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

            public virtual bool Equals(object obj)
            {
                var position = obj as DeskCellPosition;
                return position._column == _column && position._row == _row;
            }
        }
    }
}