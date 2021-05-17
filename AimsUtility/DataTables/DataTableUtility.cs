using System.Data;
using System.IO;
using System;
using GenericParsing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

// TODO: Anonymous object to data table

namespace AimsUtility.DataTables
{
    /// <summary>
    /// A set of useful functions for manipulating DataTables.
    /// </summary>
    public static class DataTableUtility
    {
        /// <summary>
        /// Creates a data
        /// </summary>
        /// <param name="CsvContents">A stream containing the CsvContents</param>
        /// <returns></returns>
        public static DataTable ParseFromCsv(string CsvContents)
        {
            var textReader = new StringReader(CsvContents);

            using(var adapter = new GenericParserAdapter(textReader))
            {
                adapter.FirstRowHasHeader = true;
                var table = adapter.GetDataTable();
                return table;
            }
        }

        /// <summary>
        /// Copies the data from the source table to the target table.
        /// They must have the same schema (columns). DATA IN THE TARGET
        /// TABLE WILL BE OVERWRITTEN.
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

        /// <summary>
        /// Returns a list of column names corresponding to the table.
        /// </summary>
        /// <param name="table">The table whose columns we are returning</param>
        /// <returns>A list of column names</returns>
        public static List<string> GetColumnNames(this DataTable table)
        {
            var columnNames = new List<string>();
            foreach(DataColumn column in table.Columns)
                columnNames.Add(column.ColumnName);
            return columnNames;
        }

        /// <summary>
        /// Converts the data table into a CSV file as a memory stream
        /// </summary>
        /// <param name="table">The table to convert</param>
        /// <param name="dateFormat">The string format of dates</param>
        /// <returns>A MemoryStream representing the data in the CSV file</returns>
        public static MemoryStream ToCSV(this DataTable table, string dateFormat = "yyyy-MM-dd")
        {
            // get columns and create stream to output
            var columns = table.GetColumnNames();
            var stream = new MemoryStream();

            // write columns to the stream
            stream.Write(Encoding.UTF8.GetBytes(String.Join(",", columns) + "\n"));

            // begin writing table data to stream
            var writer = new StreamWriter(stream);

            // iterate over rows
            foreach(DataRow row in table.Rows)
            {
                
                // this list is to store all the column values
                var outList = new List<string>();
                foreach(var colName in columns)
                {
                    var val = row[colName];

                    // convert val to date string if necessary
                    string valStr = null;
                    valStr = val == null ? "" : val.ToString();
                    if(val != null && val.GetType() == typeof(DateTime))
                        valStr = ((DateTime)val).ToString(dateFormat);

                    outList.Add(valStr);
                }

                // write the row data to the stream (need to encode it into bytes)
                var outListStr = String.Join(",", outList) + "\n";
                var outListBytes = Encoding.UTF8.GetBytes(outListStr);
                stream.Write(outListBytes);
            }
            
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Uses EPPlus's non-commercial license to convert the data table into an Xlsx ExcelPackage
        /// </summary>
        /// <param name="table">The table to be converted</param>
        /// <param name="ColumnOrder">Including this will use these column names instead of just fetching from the table. This will also preserve the order in the way you want.</param>
        /// <returns>The ExcelPackage representing the table</returns>
        public static ExcelPackage ToXlsx(this DataTable table, string[] ColumnOrder = null)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;           
            
            // initialize file
            var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("sheet1");

            var colNames = ColumnOrder == null ? table.GetColumnNames() : ColumnOrder.ToList();
            // set headers
            for(int i = 0; i < colNames.Count; i++)
            {
                ws.Cells[1, i + 1].Value = colNames[i];
            }

            // insert row data into Xlsx
            for(int i = 0; i < table.Rows.Count; i++)
            {
                for(int j = 0; j < colNames.Count; j++)
                {
                    ws.Cells[i + 2, j + 1].Value = table.Rows[i][j];
                }
            }

            return package;
        }

        /// <summary>
        /// Get a MemoryStream representing the Xlsx File created from the data table.
        /// </summary>
        /// <param name="table">The table to be converted</param>
        /// <param name="ColumnOrder">Including this will use these column names instead of just fetching from the table. This will also preserve the order in the way you want.</param>
        /// <returns>The Memory Stream representing the Xlsx file</returns>
        public static MemoryStream ToXlsxFile(this DataTable table, string[] ColumnOrder = null)
        {
            var excelPackage = table.ToXlsx(ColumnOrder);
            var memStream = new MemoryStream(excelPackage.GetAsByteArray());
            return memStream;
        }
    }
}