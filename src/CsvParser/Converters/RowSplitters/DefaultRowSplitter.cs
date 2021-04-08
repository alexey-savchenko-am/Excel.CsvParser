namespace CsvParser.Converters.RowSplitters
{
    using CsvParser.Abstract;
    using CsvParser.Abstract.Models;
    using System;
    using System.Collections.Generic;

    public class DefaultRowSplitter
        : ISplitRowStrategy
    {
        public IEnumerable<T> SplitRow<T>(string row, char separator, bool removeQuotes) 
            where T : IColumn, new()
        {
            if (string.IsNullOrWhiteSpace(row))
                yield break;

            int i = 0;
            // don't remove empty rows
            foreach (var columnName in row.Split(separator, StringSplitOptions.None))
            {
                string value = columnName;

                if (removeQuotes)
                    value = value.Replace("\"", "");

                yield return new T
                {
                    Index = i++,
                    Value = value.Trim()
                };
            }
        }
    }
}
