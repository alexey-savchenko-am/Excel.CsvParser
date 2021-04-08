namespace CsvParser.Models
{
    using CsvParser.Abstract.Models;

    internal class CsvColumn
        : IColumn
    {
        public int Index { get; set; }
        public string Value { get; set; }
    }
}
