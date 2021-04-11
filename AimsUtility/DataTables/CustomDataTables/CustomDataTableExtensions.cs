using System.Data;
using System.Linq;
using System;

namespace AimsUtility.DataTables
{
    public static class CustomDataTableExtensions
    {
        /// <summary>
        /// Copies the data from the source table to the target table.
        /// They must have the same schema (columns)
        /// </summary>
        /// <param name="SourceTable">The source of the data</param>
        /// <param name="TargetTable">The location to put the data into. ANY DATA HERE WILL BE OVERWRITTEN</param>
        /// <returns></returns>
        public static DataTable CopyDataTo(this DataTable SourceTable, DataTable TargetTable)
        {
            // clears target table and gets the row from the source
            var data = SourceTable.Select();
            TargetTable.Rows.Clear();
            
            // attempt to add rows to new table
            foreach(DataRow row in data)
                TargetTable.Rows.Add(row.ItemArray);

            return TargetTable;
        }

        /// <summary>
        /// Returns an object array that contains the primary keys
        /// that would reference this row in its parent table.
        /// </summary>
        /// <param name="Row">The row from which we will extract the primary key values.</param>
        /// <returns>The primary key values.</returns>
        public static object[] GetPrimaryKeyValues(this DataRow Row)
        {
            if(Row.Table == null)
                throw new Exception("Row has no parent table");

            // get DataColumns
            var primaryKeyColumns = Row.Table.PrimaryKey;           

            if(primaryKeyColumns == null || primaryKeyColumns.Length == 0)
                throw new Exception("Row's parent table does not have a set of primary keys");

            // convert to strings (column names)
            var pkColNames = primaryKeyColumns.Select(c => c.ColumnName).ToArray();

            // get the values from the row
            var pkValArr = pkColNames.Select(cn => Row[cn]).ToArray();

            return pkValArr;
        }
    }
}