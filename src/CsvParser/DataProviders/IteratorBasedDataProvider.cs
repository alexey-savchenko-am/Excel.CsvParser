namespace CsvParser.DataProviders
{
    using CsvParser.Abstract;
    using CsvParser.Converters.RowSplitters;

    public class IteratorBasedDataProvider
        : CsvDataProvider
    {
        public IteratorBasedDataProvider(CsvIterator iterator, char separator)
            : base(
                iterator: iterator,
                new QuotesSensitiveRowSplitter(),
                separator: separator
              )
        { }

        public IteratorBasedDataProvider(CsvIterator iterator, ISplitRowStrategy rowSplitter, char separator)
            : base(
                iterator: iterator,
                rowSplitter,
                separator: separator
              )
        { }
    }
}
