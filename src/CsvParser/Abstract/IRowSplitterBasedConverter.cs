namespace CsvParser.Abstract
{
    public interface IRowSplitterBasedConverter<TConverter, TModel>
        where TConverter : ISplitRowStrategy
    {
        TModel Convert(TConverter rowSplitter, string value, char separator = ',', bool removeQuotes = true);
    }

}
