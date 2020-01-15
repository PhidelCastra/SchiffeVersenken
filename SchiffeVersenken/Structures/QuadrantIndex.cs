using SchiffeVersenken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Structures
{
    class QuadrantIndex
    {
        public char RowIndex { get; private set; }
        public int CellIndex { get; private set; }

        /* Constructor */
        public QuadrantIndex(char row, int cell)
        {
            RowIndex = row;
            CellIndex = cell;
        }

        /// <summary>
        /// Returns row char index.
        /// </summary>
        /// <returns>Number of row index in alphabet array.</returns>
        public int GetRowIndexAsNumber()
        {
            return Array.IndexOf(Constants.Alphabet, RowIndex);
        }

    }
}
