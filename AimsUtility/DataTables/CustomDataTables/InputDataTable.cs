using System;
using System.IO;
using System.Data;
using System.Collections;

using System.Collections.Generic;

namespace DataTableUtil.DataTables{
    public class InputDataTable : DataTable{
        
        // * * * * * * * * * * Storing Exceptions * * * * * * * * * *
        public ExceptionContainer TableExceptions;



        /// <summary>
        /// Initializes to empty data table and initializes all structs
        /// that are necessary for AimsDataTable operation.
        /// </summary>
        /// <returns></returns>
        public InputDataTable() : base()
        {
            TableExceptions = new ExceptionContainer();
        }

        /// <summary>
        /// Initializes to empty data table and initializes all structs
        /// that are necessary for AimsDataTable operation. Also sets
        /// the DataTable's name.
        /// </summary>
        /// <returns></returns>
        public InputDataTable(string TableName) : base(TableName)
        {
            TableExceptions = new ExceptionContainer();
        }


        /// <summary>
        /// Copies the schema of the Template table
        /// into this table
        /// </summary>
        /// <param name="TemplateTable"></param>
        public InputDataTable(DataTable TemplateTable) : base()
        {
            TableExceptions = new ExceptionContainer();

            using (var stream = new MemoryStream())
            {
                // read the schema from the template table
                var streamWriter = new StreamWriter(stream);
                TemplateTable.WriteXmlSchema(streamWriter);

                // load the schema into this table
                stream.Position = 0;
                var streamReader = new StreamReader(stream);
                this.ReadXmlSchema(streamReader);
            }
        }

        /// <summary>
        /// Fetches the list of exceptions corresponding to a given row.
        /// </summary>
        /// <param name="Row">The row whose exceptions you're interested in.</param>
        /// <returns>A list of the row's exceptions, null if they don't exist</returns>
        public List<Exception> GetRowExceptions(DataRow Row)
        {
            return TableExceptions.GetRowExceptions(Row);
        }

        /// <summary>
        /// Gets the exceptions corresponding to a row from the 
        /// ExceptionContainer object of this table.
        /// </summary>
        /// <param name="RowIndex">The index at which we will find the row.</param>
        /// <param name="ColumnName">The column to fetch the exceptions of</param>
        /// <returns>null if there are no associated exceptions, or the associated list if it does exist.</returns>
        public List<Exception> GetColumnExceptions(int RowIndex, string ColumnName)
        {
            var row = this.Rows[RowIndex];

            return TableExceptions.GetColumnExceptions(row, ColumnName);
        }


        /// <summary>
        /// Adds an exception to the RowLevelExceptions dictionary,
        /// with the key being TargetRow. Creates a new
        /// entry if necessary, otherwise appends the exception to the
        /// list if present.
        /// </summary>
        /// <param name="Row">The row to insert the exception for.</param>
        /// <param name="Ex">The exception to insert.</param>
        public void AddRowException(DataRow Row, Exception Ex)
        {
            // we need the row to be of this table (for primary key reasons)
            if(Row.Table != this)
                throw new Exception("When adding exceptions to a row, the target row must be from the same table as the exception list");

            TableExceptions.AddRowException(Row, Ex);
        }


        /// <summary>
        /// Adds an exception to the ColumnLevelExceptions dictionary,
        /// with the key being (TargetRow, ColumnName). Creates a new
        /// entry if necessary, otherwise appends the exception to the
        /// list if present.
        /// </summary>
        /// <param name="Row">The row to insert the exception for.</param>
        /// <param name="ColumnName">The column to insert the exception for.</param>
        /// <param name="Ex">The exception to insert.</param>
        public void AddColumnException(DataRow Row, string ColumnName, Exception Ex)
        {
            // we need the row to be of this table (for primary key reasons)
            if(Row.Table != this)
                throw new Exception("When adding exceptions to a row, the target row must be from the same table as the exception list");

            TableExceptions.AddColumnException(Row, ColumnName, Ex);
        }
    }
}