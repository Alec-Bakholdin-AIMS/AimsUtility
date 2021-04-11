using System;
using System.Data;

namespace DataTableUtil.DataTables
{
    public class MismatchedValueException : Exception
    {
        public DataRow Row;
        public string ColumnName;
        public string Expected;
        public string Actual;


        /// <summary>
        /// An exception that's used when there are two rows that are 
        /// supposed to have matching values but one of them does not.
        /// In this case, the mismatched row is passed to this
        /// constructor and it's considered an exception.
        /// </summary>
        /// <param name="Row">The row that has the mismatched value.</param>
        /// <param name="ColumnName">The column in which the mismatched value was observed.</param>
        /// <param name="Expected">The value to expect (from previous rows).</param>
        /// <param name="Actual">The value that was observed, as a string.</param>
        public MismatchedValueException(DataRow Row, string ColumnName, string Expected, string Actual)
            : base($"Unexpected Mismatched value in column '{ColumnName}'. Expected: '{Expected}', Actual: '{Actual}'")
        {
            this.Row = Row;
            this.ColumnName = ColumnName;
            this.Expected = Expected;
            this.Actual = Actual;
        }

    }
}