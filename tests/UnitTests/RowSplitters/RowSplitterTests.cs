using CsvParser.Abstract.Models;
using CsvParser.Converters.RowSplitters;
using Xunit;

namespace UnitTests.RowSplitters
{
    public class RowSplitterTests
    {
        [Theory]
        [InlineData("My,Name,Is,Alex")]
        [InlineData("\"233,23\", \"634,124\"")]
        [InlineData("John, Doe, \"134,54\", \"03 jan 2012\"")]
        public void SuccessfullyParseStringWithQuotes(string str)
        {
    
            var splitter = new QuotesSensitiveRowSplitter();
            var columns = splitter.SplitRow<TestColumn>(str, ',', true);
        }


        class TestColumn : IColumn
        {
            public int Index { get; set; }
            public string Value { get; set; }
        }
    }
}
