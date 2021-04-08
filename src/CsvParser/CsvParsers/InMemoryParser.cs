namespace CsvParser.CsvParsers
{
    using CsvParser.Abstract;
    using CsvParser.Abstract.Models;
    using System.Threading.Tasks;

    class InMemoryParser<TModel>
        : CsvParser<TModel>
        where TModel : ICsvModel, new()
    {
        public InMemoryParser(
            CsvDataProvider dataProvider
         )
            : base(dataProvider)
        {}
    }
}
