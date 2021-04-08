namespace CsvParser.Abstract
{
    using CsvParser.Abstract.Models;
    using System.Collections.Generic;
    using System.Globalization;

    public interface ICsvRowConvertionStrategy<TModel>
        : ITypeConverter<IEnumerable<IColumn>, TModel>,
          ITypeConverter<IRow, TModel>,
          IRowSplitterBasedConverter<ISplitRowStrategy, TModel>
        where TModel: ICsvModel, new()

    {
        bool IsInitialized { get; }
        bool Initialize(IRow header);
    }
}
