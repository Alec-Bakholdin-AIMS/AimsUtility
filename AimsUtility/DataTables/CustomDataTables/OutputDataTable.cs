using System.Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DataTableUtil.DataTables
{
    public class OutputDataTable : DataTable
    {
        public Dictionary<object[], bool> RowValidityDict;


        /// <summary>
        /// Base Constructor. When a new row is added, it is first set to
        /// valid, then it may be modified as needed.
        /// </summary>
        /// <returns></returns>
        public OutputDataTable() : base()
        {
            RowValidityDict = new Dictionary<object[], bool>(new ExceptionDictionaryEqualityComparer());
            this.RowChanged += new DataRowChangeEventHandler(OutputDataTable_RowChanged);
        }

        /// <summary>
        /// Base Constructor but you can add a name. When a new 
        /// row is added, it is first set to valid, then it may 
        /// be modified as needed.
        /// </summary>
        /// <returns></returns>
        public OutputDataTable(string TableName) : base(TableName)
        {
            RowValidityDict = new Dictionary<object[], bool>(new ExceptionDictionaryEqualityComparer());
            this.RowChanged += new DataRowChangeEventHandler(OutputDataTable_RowChanged);
        }

        /// <summary>
        /// Copies the Schema of the TemplateTable
        /// into this OutputTable. Initializes the
        /// RowValidityDict and also makes it so that
        /// all rows added are valid by default.
        /// </summary>
        /// <param name="TemplateTable"></param>
        public OutputDataTable(DataTable TemplateTable) : base()
        {
            // rows are inserted as valid by default
            RowValidityDict = new Dictionary<object[], bool>(new ExceptionDictionaryEqualityComparer());
            this.RowChanged += new DataRowChangeEventHandler(OutputDataTable_RowChanged);
            
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
        /// Loops over the rows in the data table and
        /// if they're invalid, deletes them from the 
        /// table and the RowValidityDict. Rows that
        /// are not in the dictinoary are considered
        /// invalid.
        /// </summary>
        public void DeleteInvalidRows()
        {
            var markedForDeletion = new List<DataRow>();
            foreach(DataRow Row in this.Rows)
            {
                var pkValues = Row.GetPrimaryKeyValues();

                // either not in dictionary or valid = false
                if(!RowValidityDict.ContainsKey(pkValues) || !RowValidityDict[pkValues])
                    markedForDeletion.Add(Row); 
            }

            foreach(DataRow Row in markedForDeletion)
            {
                var pkValues = Row.GetPrimaryKeyValues();
                
                // remove from dict, if present
                if(RowValidityDict.ContainsKey(pkValues))
                    RowValidityDict.Remove(pkValues);

                // delete from table
                this.Rows.Remove(Row);
            }
        }

        /// <summary>
        /// Returns whether or not the row in quesiton is valid
        /// according to the RowValidityDict.
        /// </summary>
        /// <param name="Row">The row in question</param>
        /// <returns>True if the row is valid, false if it isn't valid or isn't present in the dictionary.</returns>
        public bool IsValidRow(DataRow Row)
        {
            if(Row.Table != this)
                throw new Exception("The row in question does not belong to this table and so its validity cannot be determined");

            // check if in dictionary
            var pkValues = Row.GetPrimaryKeyValues();
            if(!RowValidityDict.ContainsKey(pkValues))
                return false;
            
            // return dict value
            return RowValidityDict[pkValues];
        }
        
        /// <summary>
        /// Sets the validity of a row in the RowValidityDict
        /// to the specified validity. Only works if the row
        /// is already present in the dictionary
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Validity"></param>
        public void SetRowValidity(DataRow Row, bool Validity)
        {
            var pkValues = Row.GetPrimaryKeyValues();
            if(!RowValidityDict.ContainsKey(pkValues))
                throw new Exception("Specified row is not in the RowValidityDict and as such cannot have its validity changed by SetRowValidity");

            RowValidityDict[pkValues] = Validity;
        }


        /// <summary>
        /// Event handler for when a new row is inserted. In this case, it
        /// just sets the row to valid in the RowValidityDict
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OutputDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if(e.Action == DataRowAction.Add)
            {
                var pkValues = e.Row.GetPrimaryKeyValues();
                ((OutputDataTable)sender).RowValidityDict[pkValues] = true;
            }
        }
    }

    public static class OutputTableUtility
    {
        /// <summary>
        /// Adds a row to the data table, but only if the row is a member of an OutputDataTable
        /// </summary>
        /// <param name="RowCollection">The row collection of the OutputDataTable object</param>
        /// <param name="Row">The row to insert (member of an OutputDataTable)</param>
        /// <param name="Validity">Determine whether to insert it into the RowValidity dictionary as true or false</param>
        public static void Add(this DataRowCollection RowCollection, DataRow Row, bool Validity)
        {
            // check if table is an OutputDataTable
            if(Row.Table.GetType() != typeof(OutputDataTable))
                throw new Exception("Row is not a member of an OutputDataTable object");

            RowCollection.Add(Row);
        }


        /// <summary>
        /// Adds a new row to the row collection with validity.
        /// </summary>
        /// <param name="DataRowCollection"></param>
        /// <param name="RowData"></param>
        /// <param name="Validity"></param>
        public static void Add(this DataRowCollection RowCollection, object[] RowData, bool Validity)
        {
            var row = RowCollection.Add(RowData);
            if(row.Table.GetType() != typeof(OutputDataTable))
                throw new Exception("RowCollection is not a member of an OutputDataTable object");

            ((OutputDataTable)row.Table).SetRowValidity(row, Validity);
        }
    }
}