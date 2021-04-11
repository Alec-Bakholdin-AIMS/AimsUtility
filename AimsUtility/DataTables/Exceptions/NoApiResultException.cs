using System;
using System.Linq;

namespace DataTableUtil.DataTables
{
    public class NoApiResultException : Exception
    {
        public string outputColumn;
        public string[] inputValues;
        public NoApiResultException(string outputColumn, string[] inputValues) 
            : base($"No result found in API for output column {outputColumn} matching values {String.Join(", ", inputValues)}")
        {
            this.outputColumn = outputColumn;
            this.inputValues = inputValues;
        }
    }
}