namespace CsvParser.Converters.RowSplitters
{
    using CsvParser.Abstract;
    using CsvParser.Abstract.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Splits specific row of symbols as set of columns.
    /// This strategy takes into account, that csv row may contain quotes block, which may have separator symbols inside.
    /// For example, assume csv row: Bill,Gates,"entrepreneur, public figure, philanthropist", "oct 28, 1955, Seattle"
    /// The string above contains two substrings with separator symbols inside, 
    /// but they should not be perceived as separators, but as data.
    /// The strategy solve this problem, so splitter will return a set of columns containing following strings:
    /// ["Bill", "Gates", "entrepreneur, public figure, philanthropist", "oct 28, 1955, Seattle"]
    /// </summary>
    public class QuotesSensitiveRowSplitter
        : ISplitRowStrategy
    {
        public IEnumerable<T> SplitRow<T>(string row, char separator, bool removeQuotes) 
            where T : IColumn, new()
        {

            if (string.IsNullOrWhiteSpace(row))
                yield break;

            var len = row.Length;

            int index = 0;
            bool quotesDetected = false;
            int iBeg = 0;
            int iEnd = 0;

            do
            {
                var ch = row[iEnd];

                if (ch == '"')
                    quotesDetected = !quotesDetected;


                if (ch == separator || iEnd == len - 1)
                {
                    if (quotesDetected)
                    {
                        iEnd++;
                        continue;
                    }

                    var count = iEnd == len - 1 ? iEnd + 1 - iBeg : iEnd - iBeg;

                    var str = row.Substring(iBeg, count).Trim();
                    iBeg = iEnd + 1;
                    yield return new T() { Index = index++, Value = removeQuotes ? str.Trim('"') : str };
                }

                iEnd++;

            } while (iEnd < len);
        }
    }
}
