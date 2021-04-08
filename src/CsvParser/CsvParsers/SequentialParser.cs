namespace CsvParser.CsvParsers
{
    using CsvParser.Abstract;
    using CsvParser.Abstract.Models;
    using CsvParser.Converters.RowSplitters;
    using CsvParser.Models;
    using System;
    using System.Threading.Tasks;

    public class SequentialParser<TModel>
        : CsvParser<TModel>
        where TModel: ICsvModel, new()
    {
        private readonly Action<int, TModel> _lineProcessedCallback;

        public SequentialParser(
            CsvDataProvider dataProvider,
            Action<int, TModel> lineProcessedCallback   
         ) 
            : base(dataProvider)
        {
            _lineProcessedCallback = lineProcessedCallback;
        }

        protected override async Task ProcessBodyAsync(ICsvRowConvertionStrategy<TModel> csvConverter)
        {
            var iterator = _dataProvider.GetIterator();
            
            if (iterator == null)
                throw new CsvParserException("csv iterator is not set");

            var rowSplitter = _dataProvider.GetRowSplitter() ?? new QuotesSensitiveRowSplitter();

            while (await iterator.ReadLineAsync().ConfigureAwait(false))
            {
                var columns = rowSplitter.SplitRow<CsvColumn>(iterator.GetValue(), _dataProvider.Separator, true);
                var model = csvConverter.Convert(columns);

                _lineProcessedCallback.Invoke(iterator.CurrentRow, model);
            }
        }
    }
}
