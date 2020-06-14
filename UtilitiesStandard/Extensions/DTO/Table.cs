using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace Utilities.Extensions
{
    public class Table
    {
        string[,] _cells;
        int _numOfRows, _numOfColumns;

        internal Table(IEnumerable<string> cells, int numOfRows, int numOfColumns)
        {
            _cells = new string[numOfRows, numOfColumns];
            _numOfRows = numOfRows;
            _numOfColumns = numOfColumns;
            using (var cellsRator = cells.GetEnumerator())
            {
                for (int row = 0; row < numOfRows; row++)
                {
                    for (int column = 0; column < numOfColumns; column++)
                    {
                        cellsRator.MoveNext();
                        _cells[row, column] = cellsRator.Current;
                    }
                }
            }
        }

        public Table Transpose()
        {
            var transposed = new string[_numOfColumns, _numOfRows];
            for (int row = 0; row < _numOfRows; row++)
            {
                for (int column = 0; column < _numOfColumns; column++)
                {
                    transposed[column, row] = _cells[row, column];
                }
            }

            _cells = transposed;
            _numOfRows = _cells.GetLength(0);
            _numOfColumns = _cells.GetLength(1);

            return this;
        }

        public string AsExcelTable()
        {
            var sb = new StringBuilder(_numOfRows * _numOfColumns * 2);
            for (int row = 0; row < _numOfRows; row++)
            {
                for (int column = 0; column < _numOfColumns; column++)
                {
                    sb.Append(_cells[row, column]).Append('\t');
                }
                sb.Append(Global.NL);
            }

            return sb.ToString();
        }
    }

}
