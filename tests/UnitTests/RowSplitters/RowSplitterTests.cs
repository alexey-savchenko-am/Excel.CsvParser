namespace UnitTests.RowSplitters
{
    using CsvParser.Abstract;
    using CsvParser.Converters.RowSplitters;
    using System.Linq;
    using Xunit;

    public class RowSplitterTests
    {

        [Fact]
        public void SuccessfullyParseStringWithQuotes()
        {
            var splitter = new QuotesSensitiveRowSplitter();

            AssertColumns(splitter, "", 0);
            AssertColumns(splitter, "my,name, is,    Alex", 4);
            AssertColumns(splitter, "\"233,23\", \"634,124\"", 2);
            AssertColumns(splitter, "John, Doe, \"134,54\", \"03 jan 2012\"", 4);
            AssertColumns(splitter, "\",,,,,,,,,,,,,,,,,\",\",,,,,,,,,,,,\"", 2);
            AssertColumns(splitter, "hi,,eee", 3);
            AssertColumns(splitter, "hi,dsds,", 3);
            AssertColumns(splitter, ",,", 3);
            AssertColumns(splitter, ",,,", 4);
            AssertColumns(splitter, ",ddd,fgfg,", 4);
        }

        private void AssertColumns(ISplitRowStrategy splitter, string str, int expectedColumnCount)
        {
            var columns = splitter.SplitRow<TestColumn>(str, ',', true);
            Assert.Equal(expectedColumnCount, columns.Count());
        }

    }
}
