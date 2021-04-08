namespace CsvParser
{
    using System;

    public class CsvHeaderAttribute
        : Attribute
    {
        public CsvHeaderAttribute(string name)
        {
            this.ColumnName = name;
        }

        public string ColumnName { get; set; }
    }
}
