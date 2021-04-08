using System;
using System.Reflection;

namespace CsvParser.Models
{
    internal class FilterValue
    {
        public int Index { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string Label { get; set; }

        public Type Type { get; set; }
    }
}
