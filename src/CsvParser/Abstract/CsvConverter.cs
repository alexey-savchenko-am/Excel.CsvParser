namespace CsvParser.Abstract
{
    using CsvParser.Abstract.Models;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Converts row of csv data source to specified model.
    /// Specify culture inside constructor.
    /// Use Initialize method before Convert.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class CsvConverter<TModel>
        : ICsvRowConvertionStrategy<TModel>
        where TModel : ICsvModel, new()
    {
        public CultureInfo Culture { get; protected set; }
        public bool IsInitialized { get; protected set; }

   
        public CsvConverter(CultureInfo cultureInfo)
        {
            Culture = cultureInfo;
        }

        public abstract bool Initialize(IRow header);

        public abstract TModel Convert(IEnumerable<IColumn> value);

        public abstract TModel Convert(IRow value);

        public abstract TModel Convert(ISplitRowStrategy rowSplitter, string value, char separator = ',', bool removeQuotes = true);
        
        
    }
}
