namespace CsvParser.Models
{
    using CsvParser.Abstract.Models;
    using System.Collections.Generic;

    internal class CsvRow
        : IRow
    {
        public CsvRow(int index, IEnumerable<IColumn> columns)
        {
            Index = index;
            Columns = columns;
        }

        public int Index { get; }

        public IEnumerable<IColumn> Columns { get; }
    }
}
