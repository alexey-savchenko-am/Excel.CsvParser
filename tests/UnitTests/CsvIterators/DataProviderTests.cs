namespace UnitTests.CsvIterators
{
    using CsvParser.Abstract;
    using CsvParser.DataProviders;
    using CsvParser.DataProviders.Iterators;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnitTests.Helpers;
    using Xunit;

    public class DataProviderTests
        : CleanableTest
    {
        private const char Separator = ',';
        private const int RowCount = 100000;
        public int _byteCount = 0;

        private readonly TempFileGenerator _fileGenerator 
            = new TempFileGenerator();
        private readonly CsvDataProvider _provider;

        public DataProviderTests()
        {
            _byteCount = _fileGenerator.GenerateAsync(Separator, RowCount).Result;

            var iterator = new CsvFileIterator(_fileGenerator.FilePath, Encoding.UTF8);
            _provider = new IteratorBasedDataProvider(iterator, Separator);
        }


        [Fact]
        public async Task SuccessfullyReceiveHeaderFromCsvFile()
        {
            var headerRow = await _provider.GetHeaderAsync(true);
            var currentRow = _provider.GetIterator().CurrentRow;
            
            Assert.NotNull(headerRow);
            Assert.NotEmpty(headerRow.Columns);
            Assert.Equal(1, currentRow);
        }



        protected override void CleanUp()
        {
            _provider.Dispose();
            if (_fileGenerator.FileCreated)
                File.Delete(_fileGenerator.FilePath);
        }
    }
}
