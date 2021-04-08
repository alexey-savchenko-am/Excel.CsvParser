using System.Collections.Generic;

namespace CsvParser.Abstract.Models
{
    public interface IRow
    {
        int Index { get; }

        IEnumerable<IColumn> Columns { get; }
    }
}
