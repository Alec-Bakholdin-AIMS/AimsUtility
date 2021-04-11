using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AimsUtility.DataTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AimsUtilityUnitTests
{
    [TestClass]
    public class DataTableUnitTests
    {
        private static string DemoBearer = GetFieldFromJson("DemoBearerToken", "credentials.json");

        [TestMethod]
        public async Task Base_SingleRowMapping()
        {
            Assert.IsNotNull(DemoBearer, "Bearer token was not found in credentials.json");

            // input table
            var inputTable = GenerateInputTable(
                new string[]{"Customer PO"},
                new string[][]{
                    new string[]{"12345"}
                }
            );

            // output table
            var outputTable = GenerateOutputTable(new string[]{"Document ID"});

            // init connector
            var metadata = "Document ID`FALSE`Customer PO`string`";
            var connector = new DataTableConnector(metadata, DemoBearer);

            var postConnection = await connector.ConnectTables(inputTable, outputTable);

            // assert row count is equal (1 each) and that the values for input and output match
            Assert.AreEqual(inputTable.Rows.Count, postConnection.Rows.Count, $"Row count mismatch");  // make sure only one row
            Assert.AreEqual((string)inputTable.Rows[0]["Customer PO"], (string)postConnection.Rows[0]["Document ID"], $"DataTableConnector produced data that didn't match the input."); // make sure the values are equal
        }

        [TestMethod]
        public async Task Api_SingleSubstitution()
        {
            // create tables
            var inputTable = GenerateInputTable(new string[]{"Order"}, new string[][]{new string[]{"103862"}});
            var outputTable = GenerateOutputTable(new string[]{"Account"});

            // create connector
            var metadataStr = GenerateMetadataRow("Account", false, "{API:https://apiwest.aims360.rest/orders/v1.0/orders?$filter=order eq '[I:Order]'}(value[0].customerAccount)", "string", "");
            var connector = new DataTableConnector(metadataStr, DemoBearer);

            var postConnection = await connector.ConnectTables(inputTable, outputTable);
            Assert.AreEqual(inputTable.Rows.Count, postConnection.Rows.Count, "Mismatched row count");
            Assert.AreEqual("SHPFY", (string)postConnection.Rows[0]["Account"]);
        }

        [TestMethod]
        public async Task Api_MultipleSubstitutions()
        {
            // create tables
            var inputTable = GenerateInputTable(new string[]{"Style", "Color", "Size"}, new string[][]{new string[]{"C-ALL-HOL", "SILVR", "SM"}});
            var outputTable = GenerateOutputTable(new string[]{"UPC"});

            // create connector
            var metadataStr = GenerateMetadataRow("UPC", false, "{API:https://apiwest.aims360.rest/StyleColors/v1.1/StyleColors?$filter=style eq '[I:Style]' and color eq '[I:Color]'&$expand=sizes($filter=sizeName eq '[I:Size]')}(value[0].sizes[0].upc)", "string", "");
            var connector = new DataTableConnector(metadataStr, DemoBearer);

            var postConnection = await connector.ConnectTables(inputTable, outputTable);
            Assert.AreEqual(inputTable.Rows.Count, postConnection.Rows.Count, "Mismatched row count");
            Assert.AreEqual("810014629179", (string)postConnection.Rows[0]["UPC"]);
        }

        [TestMethod]
        public async Task Exception_MismatchedValueException()
        {
            // input table + set primary key to 'Order'
            var inputTable = GenerateInputTable(
                    new string[]{"Order", "Entered", "Style", "Type"},
                    new string[][]{
                        new string[]{"1234", "2021-03-28", "B10-FREDDIE", "one"},
                        new string[]{"1234", "2021-03-29", "B11-FREDDIE", "two"}
                    },
                    new string[]{"Order", "Style"}
                );
            var orderDataColumn = GetColumnFromName(inputTable, "Order");

            // output table + set primary key to 'Order'
            var outputTable = GenerateOutputTable(new string[]{"Order", "Entered"});

            // create connector
            var metadataStr1 = GenerateMetadataRow("Order", false, "Order", "string", "");
            var metadataStr2 = GenerateMetadataRow("Entered", false, "Entered", "string", "");
            var metadataStr = $"{metadataStr1}~{metadataStr2}";
            var connector = new DataTableConnector(metadataStr, DemoBearer);

            // connect tables
            var postConnection = await connector.ConnectTables(inputTable, outputTable);

            // assertions
            var exceptionList = inputTable.GetColumnExceptions(1, "Entered");
            Assert.AreEqual(1, exceptionList?.Count, "Expected one MismatchedHeaderException");
            var ex = exceptionList[0];
            Assert.IsInstanceOfType(ex, typeof(MismatchedValueException));
            var ex1 = (MismatchedValueException)ex;
            Assert.AreSame(ex1.Row, inputTable.Rows[1]);
            Assert.AreEqual(ex1.Expected, "2021-03-28");
            Assert.AreEqual(ex1.Actual, "2021-03-29");
        }

        [TestMethod]
        public void Utility_GenerateInputOutputTables()
        {
            // create metadata to pass to connector
            var metaArr = new string[]{
                GenerateMetadataRow("Order", false, "Order", "string", ""),
                GenerateMetadataRow("Entered", false, "Entered", "string", ""),
                GenerateMetadataRow("UPC", false, "{API:https://apiwest.aims360.rest/StyleColors/v1.1/StyleColors?$filter=style eq '[I:Style]' and color eq '[I:Color]'&$expand=sizes($filter=sizeName eq '[I:Size]')}(value[0].sizes[0].upc)", "string", ""),
                GenerateMetadataRow("HEADER", true, "", "", ""),
                GenerateMetadataRow("HEADER", true, "", "", ""),
                GenerateMetadataRow("HEADER", true, "", "", "")
            };
            var metadataStr = String.Join("~", metaArr);
            var connector = new DataTableConnector(metadataStr, DemoBearer);
            
            // generate tables
            var inputTable  = connector.GenerateEmptyInputTable("InputTable");
            var outputTable = connector.GenerateEmptyOutputTable("OutputTable");

            // create lists of the column names, sorted
            var InputExpected   = (new string[]{"Order", "Entered", "Style", "Color", "Size"}).OrderBy(e => e).ToList();
            var OutputExpected  = (new string[]{"Order", "Entered", "UPC", "HEADER", "HEADER1", "HEADER2"}).OrderBy(e => e).ToList();
            var InputActual     = inputTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).OrderBy(e => e).ToList();
            var OutputActual    = outputTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).OrderBy(e => e).ToList();

            // test for equality
            var input = Enumerable.SequenceEqual(InputExpected, InputActual);
            var output = Enumerable.SequenceEqual(OutputExpected, OutputActual);

            // run test
            Assert.IsTrue(input, $"Input columns were incorrectly generated. \nExpected: {String.Join(", ", InputExpected)}\nActual: {String.Join(", ", InputActual)}");
            Assert.IsTrue(input, $"Output columns were incorrectly generated. \nExpected: {String.Join(", ", OutputExpected)}\nActual: {String.Join(", ", OutputActual)}");
        }

        [TestMethod]
        public void Utility_CreateInputDataTableFromCopy()
        {
            // create template table
            var templateTable = new DataTable("Template");
            templateTable.Columns.Add("col1", typeof(string));
            templateTable.Columns.Add("col2", typeof(int));

            // validate that input and template are the same, column-wise
            var inputTable = new InputDataTable(templateTable);
            var templateColumns = templateTable.Columns.Cast<DataColumn>().Select(c => (c.ColumnName, c.DataType)).ToList();
            var inputColumns = inputTable.Columns.Cast<DataColumn>().Select(c => (c.ColumnName, c.DataType)).ToList();

            var equalTest = Enumerable.SequenceEqual(templateColumns, inputColumns);

            Assert.IsTrue(equalTest, $"Input and template columns are not identical.");
        }

        [TestMethod]
        public void Utility_CopyDataTo()
        {
            // create data
            var table1 = new DataTable("Table");
            table1.Columns.Add("col1", typeof(string));
            table1.Rows.Add(new object[]{"data1"});
            table1.Rows.Add(new object[]{"data2"});
            table1.Rows.Add(new object[]{"data3"});

            // copy data over
            var table2 = new DataTable();
            table2.Columns.Add("col1", typeof(string));
            table1.CopyDataTo(table2);

            // validate data
            var table1Data = table1.Rows.Cast<DataRow>().Select(r => (string)r["col1"]).ToList();
            var table2Data = table2.Rows.Cast<DataRow>().Select(r => (string)r["col1"]).ToList();

            var sequenceEqual = Enumerable.SequenceEqual(table1Data, table2Data);
            Assert.IsTrue(sequenceEqual, $"Data was not copied correctly.\nExpected: {String.Join(", ", table1Data)}\nActual: {String.Join(", ", table2Data)}");            
        }

        [TestMethod]
        public void Utility_DictionaryObjectArrayAccessibility(){
            var newDict = new Dictionary<object[], string>(new ExceptionDictionaryEqualityComparer());
            newDict.Add(new object[]{"ok", "two"}, "ten");

            // check accessible by new object array with same values
            var testVal = newDict[new object[]{"ok", "two"}];
            Assert.AreEqual("ten", testVal);

            // check that incorrect object array returns incorrect value
            var testVal2 = newDict.ContainsKey(new object[]{"ok"});
            Assert.IsFalse(testVal2);
        }

        [TestMethod]
        public async Task Exception_NoApiResultException(){
            // set up tables
            var inputTable = GenerateInputTable(new string[]{"one"}, new string[][]{new string[]{"ASDB"}, new string[]{"PAPDA"}});
            var outputTable = GenerateOutputTable(new string[]{"two"});
            inputTable.PrimaryKey = new DataColumn[]{inputTable.Columns[0]};

            // set up connector
            var metadataString = GenerateMetadataRow("two", false, "{API:https://apieast.aims360.rest/customers/v1.0/customers?$filter=customerAccount eq 'ASDB'}(value[0].customerAccount)", "string", "");
            var connector = new DataTableConnector(metadataString, DemoBearer);

            // run the connector
            var postConnection = await connector.ConnectTables(inputTable, outputTable);
            var rowExceptions = inputTable.GetRowExceptions(inputTable.Rows[0]);

            // test that there's one exception (should be NoApiResultException)
            Assert.AreEqual(1, rowExceptions.Count);
            
            // test that the exception is a NoApiResultException
            Assert.IsInstanceOfType(rowExceptions[0], typeof(NoApiResultException));
        }

        [TestMethod]
        public void Utility_OutputDataTableValidity()
        {
            var outputTable = GenerateOutputTable(new string[]{"col1"});
            outputTable.PrimaryKey = new DataColumn[]{outputTable.Columns[0]};
            var one = outputTable.Rows.Add(new object[]{"test"});
            var two = outputTable.Rows.Add(new object[]{"testing"});

            outputTable.SetRowValidity(one, false);

            Assert.AreEqual(2, outputTable.Rows.Count);
            Assert.IsFalse(outputTable.IsValidRow(one));
            Assert.IsTrue (outputTable.IsValidRow(two));

            outputTable.DeleteInvalidRows();

            Assert.AreEqual(1, outputTable.Rows.Count);
            Assert.AreEqual("testing", (string)outputTable.Rows[0]["col1"]);
        }









        // * * * * * * * * * * Helper functions * * * * * * * * * *



        /// <summary>
        /// Gets the field value from a json that is found
        /// at a local filepath, relative of course
        /// </summary>
        /// <param name="fieldName">The field of the json to fetch (currently only works at one level deep, cannot do nested objects)</param>
        /// <param name="filepath">The filepath to the json field</param>
        /// <returns></returns>
        private static string GetFieldFromJson(string fieldName, string filepath)
        {
            var contents = File.ReadAllText(filepath);
            var json = (JObject)JsonConvert.DeserializeObject(contents);
            return (string)json[fieldName];
        }

        /// <summary>
        /// Create a table of strings in one line with the information provided in the
        /// RowData 2-D string array. Uses the column names in the ColNames column.
        /// Inserts no data if the RowData array is null or empty.
        /// </summary>
        /// <param name="ColNames">The column names to use</param>
        /// <param name="RowData">The string data to insert into the table</param>
        /// <param name="PrimaryKeyColumnNames">An optional list of column names to be used as primary keys. If null, uses the first column</param>
        /// <returns>The resultant data table</returns>
        private static InputDataTable GenerateInputTable(string[] ColNames, string[][] RowData, string[] PrimaryKeyColumnNames = null)
        {

            // create table with specified column names (strings)
            var createdTable = new InputDataTable();
            ColNames.ToList().ForEach(el => createdTable.Columns.Add(new DataColumn(el, typeof(string))));
            
            // add primary keys to table if necessary
            if(PrimaryKeyColumnNames?.Length > 0)
            {
                var pkCols = PrimaryKeyColumnNames.Select(name => createdTable.Columns[name]).ToArray();
                createdTable.PrimaryKey = pkCols;
            }
            else
            {
                createdTable.PrimaryKey = new DataColumn[]{createdTable.Columns[0]};
            }

            if(RowData == null)
                return createdTable;
            // iterate over each row
            foreach(string[] inputRow in RowData)
            {
                // create new row
                var rowToInsert = createdTable.NewRow();

                // insert data into the row
                for(int i = 0; i < inputRow.Length; i++)
                    rowToInsert[i] = inputRow[i];
                
                createdTable.Rows.Add(rowToInsert);
            }

            return createdTable;
        }

        /// <summary>
        /// Creates an OutputDataTable object with the column
        /// names listed in ColNames. All the types are strings,
        /// as of now, but that will change.
        /// </summary>
        /// <param name="ColNames">The column names to create the table with.</param>
        /// <param name="TableName">The table name to be given to the output table</param>
        /// <param name="PrimaryKeyColumnNames">An optional list of column names to be used as primary keys. Otherwise, uses the first column</param>
        /// <returns>The OutputDataTable object.</returns>
        private static OutputDataTable GenerateOutputTable(string[] ColNames, string TableName = "Output Table", string[] PrimaryKeyColumnNames = null)
        {
            var table = new OutputDataTable(TableName);
            ColNames.ToList().ForEach(el => table.Columns.Add(new DataColumn(el, typeof(string))));

            // add primary keys to table if necessary
            if(PrimaryKeyColumnNames?.Length > 0)
            {
                var pkCols = PrimaryKeyColumnNames.Select(name => table.Columns[name]).ToArray();
                table.PrimaryKey = pkCols;
            }
            else
            {
                table.PrimaryKey = new DataColumn[]{table.Columns[0]};
            }

            return table;
        }


        /// <summary>
        /// Inserts the ` character between each element in the parameters
        /// </summary>
        /// <param name="OutputColumn"></param>
        /// <param name="Skip"></param>
        /// <param name="InputFormula"></param>
        /// <param name="DataType"></param>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        private static string GenerateMetadataRow(
            string OutputColumn,
            bool Skip,
            string InputFormula,
            string DataType,
            string Mapping
        )
        {
            string SkipStr = Skip ? "TRUE" : "FALSE";

            return $"{OutputColumn}`{Skip}`{InputFormula}`{DataType}`{Mapping}";
        }


        /// <summary>
        /// Gets a reference to the column whose name matches the name in
        /// the ColumnName parameter.
        /// </summary>
        /// <param name="Table">The table whose columns we are to search.</param>
        /// <param name="ColumnName">The name of the column to search for.</param>
        /// <returns>The column name whose name is ColumnName, null if it doesn't exist</returns>
        private static DataColumn GetColumnFromName(DataTable Table, string ColumnName)
        {
            foreach(DataColumn col in Table.Columns)
            {
                if(col.ColumnName == ColumnName)
                    return col;
            }

            // didn't find any columns with matching name
            return null;
        }
    }
}

