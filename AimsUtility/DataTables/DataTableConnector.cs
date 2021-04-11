using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AimsUtility.Api;

namespace AimsUtility.DataTables{
    public class DataTableConnector{

        // * * * * * * * * * * Metadata variables * * * * * * * * * *
        public string Metadata;
        public string[] MetadataRows;
        public List<MetadataObject> MetadataObjects;
        public Dictionary<string, MetadataObject> MetadataObjectsByOutput;


        // * * * * * * * * * * Error Handling * * * * * * * * * *
        public Dictionary<DataRow, List<Exception>> ExceptionsByDataRow;
    

        // * * * * * * * * * * Private variables * * * * * * * * * *
        private ApiClient Api;
        private Action<string> LoggingFunction = null;


        /// <summary>
        /// Constructor that initializes the metadata of the connector
        /// </summary>
        /// <param name="Metadata">Serialized table described in the readme. Columns are split by backtick (`) and rows are split by tilda (~)</param>
        public DataTableConnector(string Metadata, string BearerToken, string ColDelim = "`", string RowDelim = "~")
        {
            // init variables
            this.Api                     = new ApiClient(BearerToken);
            this.Metadata                = Metadata;
            this.MetadataRows            = Metadata.Split("~");
            this.MetadataObjects         = new List<MetadataObject>();
            this.MetadataObjectsByOutput = new Dictionary<string, MetadataObject>();
            this.ExceptionsByDataRow     = new Dictionary<DataRow, List<Exception>>();


            // popoulate data structures
            foreach(string rowStr in MetadataRows)
            {
                var metaObj = new MetadataObject(rowStr, this.Api);
                this.MetadataObjects.Add(metaObj);
                this.MetadataObjectsByOutput[metaObj.OutputColumn] = metaObj;
            }
        }

        
        
        /// <summary>
        /// Constructor that initializes the metadata of the connector. Also allows the user
        /// to specify how many threads the API will have active at once (so as not to overload
        /// servers)
        /// </summary>
        /// <param name="Metadata">Serialized table described in the readme. Columns are split by backtick (`) and rows are split by tilda (~)</param>
        /// <param name="BearerToken">The token used for AIMS360 authorization</param>
        /// <param name="MaxNumThreads">The maximum number of threads that can be waiting for a response from the API at once (sempahore behavior)</param>
        public DataTableConnector(string Metadata, string BearerToken, int MaxNumThreads)
        {
            // init variables
            this.Api                     = new ApiClient(BearerToken, MaxNumThreads);
            this.Metadata                = Metadata;
            this.MetadataRows            = Metadata.Split("~");
            this.MetadataObjects         = new List<MetadataObject>();
            this.MetadataObjectsByOutput = new Dictionary<string, MetadataObject>();


            // populate data structures
            foreach(string rowStr in MetadataRows)
            {
                var metaObj = new MetadataObject(rowStr, this.Api);
                this.MetadataObjects.Add(metaObj);
                this.MetadataObjectsByOutput[metaObj.OutputColumn] = metaObj;
            }
        }


        private int rowCount = 0;

        /// <summary>
        /// Moves data from the input to the output, based on the metadata
        /// provided in the constructor. Returns the output table
        /// </summary>
        /// <param name="InputTable">The input DataTable, populated with data</param>
        /// <param name="OutputTable">The output DataTable, which will be copied over to an OutputDataTable and then have data inserted into it. The output parameter will not be modified.</param>
        /// <returns>The output data table as an OutputDataTable</returns>
        public async Task<OutputDataTable> ConnectTables(InputDataTable InputTable, OutputDataTable OutputTable)
        {
            // start row tasks
            int j = 0;
            rowCount = InputTable.Rows.Count;
            
            // copy output columns to new table
            var aimsOutputTable = new OutputDataTable(OutputTable);
            
            var taskList = new List<Task<(DataRow, DataRow)>>();
            foreach(DataRow inputRow in InputTable.Rows)
            {
                var outputRow = aimsOutputTable.NewRow();
                LoggingFunction?.Invoke($"Started row {++j}");
                taskList.Add(ConnectRows(inputRow, outputRow, j));
            }

            // await all tasks
            var numTasks = taskList.Count;
            while(taskList.Count > 0)
            {
                var index = Task.WaitAny(taskList.ToArray());
                (DataRow inputRow, DataRow outputRow) = await taskList[index];
                try{
                    aimsOutputTable.Rows.Add(outputRow);
                }catch(ConstraintException){
                    // this means that there is a row that exists with the same primary key

                    // find the row
                    var keyValues = aimsOutputTable.PrimaryKey.Select(col => outputRow[col.ColumnName]).ToArray();
                    var originalRow = aimsOutputTable.Rows.Find(keyValues);
                    
                    // get the differences between the rows
                    var rowDiscrepancies = CheckIfRowsMatch(originalRow, outputRow, inputRow);
                    rowDiscrepancies?.ForEach(ex => InputTable.AddColumnException(ex.Row, ex.ColumnName, ex));
                }catch(NoNullAllowedException){
                    // allow this exception to occur. This happens when we have a column exception
                    // that involves the output's primary key. For example, NoApiResultException
                    LoggingFunction?.Invoke("ERROR: output row could not be inserted because a column exception occurred in a primary key column");
                }

                // remove so waitany doesn't wait on this again
                taskList.RemoveAt(index);
            }


            return aimsOutputTable;
        }

        
        
        /// <summary>
        /// Moves data from one row to another.
        /// We make this a task so we can parallelize rows.
        /// </summary>
        /// <param name="inputRow">The row we're taking from</param>
        /// <param name="outputRow">The row we're putting data into</param>
        /// <param name="RowNumber">The row number, used for logging purposes</param>
        /// <returns>The input row and the output row as a tuple: (inputRow, outputRow)</returns>
        public async Task<(DataRow, DataRow)> ConnectRows(DataRow inputRow, DataRow outputRow, int RowNumber)
        {
            foreach(MetadataObject obj in this.MetadataObjects)
            {
                if(obj.Skip)
                    continue;

                if(obj.IsApiCall)
                {
                    // make the api call after getting the input values corresponding to the dependent values
                    var dependentColValues = obj.DependentColumns.Select(col => (string)inputRow[col.col]).ToArray();

                    try{
                        var apiValue = await obj.GetApiValue(dependentColValues);
                        if(apiValue == null) // no value found in JSON matching the MetadataObject's JSON path
                        {
                            var ex = new NoApiResultException(obj.OutputColumn, dependentColValues);
                            ((InputDataTable)inputRow.Table).AddRowException(inputRow, ex);
                        }
                        else
                        {
                            outputRow[obj.OutputColumn] = apiValue;
                        }
                    }catch(HttpRequestException ex){ // handle messed up HTTP requests
                        ((InputDataTable)inputRow.Table).AddRowException(inputRow, ex);
                    }
                }
                else
                {
                    // if the input formula is an empty string, the column won't exist, so we just set it to empty string
                    object valueToInsert = obj.InputFormula == "" ? "" : inputRow[obj.InputFormula];

                    // map the value, if possible
                    valueToInsert = obj.GetMappedData((string)valueToInsert);

                    outputRow[obj.OutputColumn] = valueToInsert;
                }
            }

            lock(this){
                LoggingFunction?.Invoke($"Finished row {RowNumber}");
            }
            return (inputRow, outputRow);
        }







        /// <summary>
        /// Checks to see if the *values* of the rows match each other.
        /// As of now, uses basic equality, but this might have to change 
        /// in the future, perhaps using MetadataObjects for data types?
        /// Throws an exception if the origin tables are not the same.
        /// </summary>
        /// <param name="FirstRow">First Row, the one that's already in the table</param>
        /// <param name="SecondRow">Second Row, the one we wanted to insert into the table</param>
        /// <param name="InputRow">The input row we got the data from, to be passed to the MismatchedValueException constructor</param>
        /// <returns>null if the rows match, otherwise returns a list of MismatchedValueException objects, with One as the target row.</returns>
        private List<MismatchedValueException> CheckIfRowsMatch(DataRow FirstRow, DataRow SecondRow, DataRow InputRow)
        {
            // not from the same table
            //TODO: check to see if Target.table is null if it's not inserted yet
            if(FirstRow.Table != SecondRow.Table)
                throw new Exception("Encounterd two rows that were from different tables when comparing for equality");


            // loop over the columns and check the data now
            var exceptionList = new List<MismatchedValueException>();
            foreach(DataColumn col in FirstRow.Table.Columns)
            {
                var colName = col.ColumnName;
                if(FirstRow[colName] != SecondRow[colName])
                {
                    var e = new MismatchedValueException(
                            InputRow, // origin of data
                            colName,  // column where mismatch found
                            FirstRow [colName].ToString(), // column already present in table
                            SecondRow[colName].ToString()  // column that we failed to insert
                        );
                    exceptionList.Add(e);
                }
            }

            // no mismatches were found, return null
            if(exceptionList.Count == 0)
                return null;
            // otherwise, return the mismatches
            else
                return exceptionList;
        }

        
        /// <summary>
        /// Uses the metadata objects created in the constructor
        /// to determine which columns of the input data we need
        /// and creates a table with all those values stored.
        /// </summary>
        /// <param name="TableName">The name to give the resulting table</param>
        /// <returns>The data table</returns>
        public InputDataTable GenerateEmptyInputTable(string TableName)
        {
            // get the unique columns to put into the table
            var uniqueColumns = new List<(string col, Type type)>();
            foreach(var obj in MetadataObjects)
                uniqueColumns = uniqueColumns.Union(
                    obj.DependentColumns.Where(o => o.col != null && o.col != "" )
                ).ToList();

            // create the actual table
            var table = new InputDataTable();
            foreach((string col, Type type) in uniqueColumns)
                table.Columns.Add(col, type);
            
            return table;
        }


        /// <summary>
        /// Creates the output table with columns from the provided metadata.
        /// The only thing that might be weird is that duplicate columns will
        /// have an integer appended to them (starting from 1 and going upwards).
        /// The first header will have no integer (e.g HEADER, HEADER1, HEADER2)
        /// </summary>
        /// <param name="TableName">The name to give the resulting table</param>
        /// <returns>The table, with duplicate columns having an integer appended to them to avoid column collision.</returns>
        public OutputDataTable GenerateEmptyOutputTable(string TableName)
        {
            var columns = MetadataObjects.Select(obj => (obj.OutputColumn, obj.DataType)).ToList();
            var table = new OutputDataTable();
            foreach((string col, Type type) in columns)
            {
                // avoid duplicate columns by appending numbers
                var columnName = col;
                var count = 0;
                while(table.Columns.Contains(columnName))
                    columnName = col + (++count).ToString();

                // add the column
                table.Columns.Add(columnName, type);
            }

            return table;
        }






        /// <summary>
        /// Sets a logging function so the data connector can output data.  
        /// </summary>
        /// <param name="func">The function, which takes a string as input and whose return type is void </param>
        public void SetLoggingFunction(Action<string> func)
        {
            this.LoggingFunction = func;
        }
    }
}