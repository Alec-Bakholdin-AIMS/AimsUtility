using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace DataTableUtil.DataTables
{

    public class ExceptionContainer
    {
        // * * * * * * * * * * Exception Dictionaries * * * * * * * * * *
        private Dictionary<object[], List<Exception>> ExceptionsByRow;
        private Dictionary<object[], List<Exception>> ExceptionsByColumn; // this is effectively the previous dictionary but with the column name appended to the key
        

        public ExceptionContainer()
        {
            ExceptionsByRow     = new Dictionary<object[], List<Exception>>(new ExceptionDictionaryEqualityComparer());
            ExceptionsByColumn  = new Dictionary<object[], List<Exception>>(new ExceptionDictionaryEqualityComparer());
        }

        /// <summary>
        /// Function that returns the list of exceptions corresponding to the row
        /// passed as a parameter. First extracts the primary key based on parent's
        /// primary key columns and uses that as a key for the dictionary.
        /// </summary>
        /// <param name="Row">The row to extract the Exceptions of</param>
        /// <returns>The list of exceptions corresponding to the row, null if the row has no list present.</returns>
        public List<Exception> GetRowExceptions(DataRow Row)
        {
            var pkValArr = Row.GetPrimaryKeyValues();
            if(!ExceptionsByRow.ContainsKey(pkValArr))
                return null;

            return ExceptionsByRow[pkValArr];
        }

        /// <summary>
        /// Function that returns the list of exceptions corresponding to the
        /// primary key values passed in the paramater
        /// </summary>
        /// <param name="PrimaryKeyValues">Primary key array corresponding to the row that you wish to retrieve the exceptions of.</param>
        /// <returns>The list of exceptions corresponding to the row. Null if none present.</returns>
        public List<Exception> GetRowExceptions(object[] PrimaryKeyValues)
        {
            if(!ExceptionsByRow.ContainsKey(PrimaryKeyValues))
                return null;

            return ExceptionsByRow[PrimaryKeyValues];
        }


        /// <summary>
        /// Gets the list of exceptions corresponding to a column
        /// of a given row.
        /// </summary>
        /// <param name="Row">The target row.</param>
        /// <param name="ColumnName">The target column.</param>
        /// <returns>The list of exceptions. Null if none present.</returns>
        public List<Exception> GetColumnExceptions(DataRow Row, string ColumnName)
        {
            var pkValArr = Row.GetPrimaryKeyValues();
            var colAppended = pkValArr.Append(ColumnName).ToArray();

            if(!ExceptionsByColumn.ContainsKey(colAppended))
                return null;

            return ExceptionsByColumn[colAppended];
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
            var pkValArr = Row.GetPrimaryKeyValues();
            if(!ExceptionsByRow.ContainsKey(pkValArr))
                ExceptionsByRow[pkValArr] = new List<Exception>();

            ExceptionsByRow[pkValArr].Add(Ex);
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
            var pkValArr = Row.GetPrimaryKeyValues();
            var colAppended = pkValArr.Append(ColumnName).ToArray();
            
            if(!ExceptionsByColumn.ContainsKey(colAppended))
                ExceptionsByColumn[colAppended] = new List<Exception>();

            ExceptionsByColumn[colAppended].Add(Ex);
        }









        // * * * * * * * * * * Getter/Setter Operators * * * * * * * * * *
        public List<Exception> this[DataRow Row]
        {
            get {
                return GetRowExceptions(Row);
            }
        }

        public List<Exception> this[DataRow Row, string ColumnName]
        {
            get {
                return GetColumnExceptions(Row, ColumnName);
            }
        }
    }








    /// <summary>
    /// Custom class that allows me to use the
    /// primary key arrays (the arrays with the
    /// values of the primary key columns) as 
    /// the key to the dictionary containing
    /// all the error information. Uses
    /// .ToString() to get the unique value
    /// of each value, and computes a hash using
    /// the .ToString() value of each object.
    /// </summary>
    public class ExceptionDictionaryEqualityComparer : IEqualityComparer<object[]>{
        

        /// <summary>
        /// Tests for equality between two object arrays by performing 
        /// element-wise comparison between .ToString() value of each.
        /// </summary>
        /// <param name="one">The first array in the test for equality</param>
        /// <param name="two">The second array in the test for equality</param>
        /// <returns></returns>
        public bool Equals(object[] one, object[] two)
        {
            // ensure equal lengths
            if(one.Length != two.Length)
                return false;

            // do element-wise value comparison using .ToString()
            for(int i = 0; i < one.Length; i++)
            {
                if(one.ToString() != two.ToString())
                    return false;
            }

            return true;
        }

        /// <summary>
		/// Gets the hash code for the contents of the array since the default hash code
		/// for an array is unique even if the contents are the same.
		/// </summary>
		/// <remarks>
		/// See Jon Skeet (C# MVP) response in the StackOverflow thread 
		/// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
		/// </remarks>
		/// <param name="array">The array to generate a hash code for.</param>
		/// <returns>The hash code for the values in the array.</returns>
		public int GetHashCode(object[] array)
		{
			// if non-null array then go into unchecked block to avoid overflow
			if (array != null)
			{
				unchecked
				{
					int hash = 17;

					// get hash code for all items in array
					foreach (var item in array)
						hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);

					return hash;
				}
			}

			// if null, hash code is zero
			return 0;
		}
    }

}