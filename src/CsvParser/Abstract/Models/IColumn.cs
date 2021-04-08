namespace CsvParser.Abstract.Models
{
    public interface IColumn
    {
        public int Index { get; set; }
        public string Value { get; set; }
    }
}
