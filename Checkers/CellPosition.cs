namespace Checkers
{
    public partial class Cell
    {
        public class CellPosition
        {
            private readonly int _column;
            private readonly int _row;


            public int Get_row()
            {
                return _row;
            }

            public int Get_column()
            {
                return _column;
            }

            public int[] Get_position()
            {
                int[] position = {_column, _row};
                return position;
            }

            public CellPosition(int column, int row)
            {
                _column = column;
                _row = row;
            }

            public new virtual bool Equals(object obj)
            {
                return obj is CellPosition position && (position._column == _column && position._row == _row);
            }
        }
    }
}