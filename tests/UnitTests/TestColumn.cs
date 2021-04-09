namespace UnitTests
{
    using CsvParser.Abstract.Models;
    using System.Collections.Generic;

    class TestColumn 
        : IColumn
    {
        public int Index { get; set; }
        public string Value { get; set; }
    }

    internal class TestRow
    : IRow
    {
        public TestRow(int index, IEnumerable<IColumn> columns)
        {
            Index = index;
            Columns = columns;
        }

        public int Index { get; }

        public IEnumerable<IColumn> Columns { get; }
    }
}
