using System.Data;
using System.IO;
using System;
using GenericParsing;

namespace AimsUtility.DataTables
{
    public static class DataTableUtillity
    {
        /// <summary>
        /// Creates a data
        /// </summary>
        /// <param name="CsvContents">A stream containing the CsvContents
        /// <returns></returns>
        public static DataTable ParseFromCsv(string CsvContents)
        {
            var textReader = new StringReader(CsvContents);

            using(var adapter = new GenericParserAdapter(textReader))
            {
                var table = adapter.GetDataTable();
                return table;
            }
        }
    }
}