namespace CsvParser.Abstract
{
    using CsvParser.Abstract.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Splits specific row of symbols as set of columns.
    /// </summary>
    public interface ISplitRowStrategy
    {
        IEnumerable<T> SplitRow<T>(string row, char separator, bool removeQuotes)
            where T: IColumn, new();
    }
}
