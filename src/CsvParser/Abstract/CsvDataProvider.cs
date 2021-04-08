namespace CsvParser.Abstract
{
    using CsvParser.Models;
    using CsvParser.Converters.RowSplitters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CsvParser.Abstract.Models;

    public abstract class CsvDataProvider
        : IDisposable
    {
        protected readonly CsvIterator _iterator;
        protected readonly ISplitRowStrategy _rowSplitter;
        protected readonly char _separator;

        #region Constructors
        public CsvDataProvider(CsvIterator iterator)
            : this(
                  iterator: iterator,
                  new QuotesSensitiveRowSplitter(),
                  separator: ','
            )
        { }

        public CsvDataProvider(CsvIterator iterator, char separator)
            : this(
              iterator: iterator,
              new QuotesSensitiveRowSplitter(),
              separator: separator
            )
        { }

        public CsvDataProvider(CsvIterator iterator, ISplitRowStrategy rowSplitter, char separator)
        {
            _iterator = iterator;
            _rowSplitter = rowSplitter;
            _separator = separator;
        }
        #endregion


        public CsvIterator GetIterator() => _iterator;

        public ISplitRowStrategy GetRowSplitter() => _rowSplitter;

        public char Separator { get => _separator;}


        /// <summary>
        /// Get all data from csv file
        /// Callback invokes each getting of row
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="removeQuotes"></param>
        /// <returns></returns>
        public virtual async Task<CsvDataProvider> GetDataAsync(Action<IRow> callback, bool invokeOnHeaderRow = false, bool removeQuotes = true)
        {
            var header = 
                await GetHeaderAsync(removeQuotes)
                      .ConfigureAwait(false);

            if(invokeOnHeaderRow)
                callback.Invoke(header);

            var body = GetBodyAsync(header.Index, removeQuotes)
                       .ConfigureAwait(false);

            await foreach(var bodyRow in body)
            {
                callback.Invoke(bodyRow);
            }

            return this;
        }

        /// <summary>
        /// Extract header row from csv data source
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IRow> GetHeaderAsync(bool removeQuotes = true)
        {

            // set iterator to the begining of data set
            if (!await _iterator
                .ResetAsync()
                .ConfigureAwait(false))
                throw new CsvParserException("can not reset csv iterator");


            // find csv header row
            // loop it, due empty row possibility
            while (await _iterator
                .ReadLineAsync()
                .ConfigureAwait(false))
            {
               var columns = _rowSplitter.SplitRow<CsvColumn>(_iterator.GetValue(), _separator, removeQuotes);
               
               if(columns.Any()) 
                    return new CsvRow(_iterator.CurrentRow, columns);
            }

            throw new CsvParserException("header row was not found");
        }

        /// <summary>
        /// Extract body rows from csv data source
        /// </summary>
        /// <param name="bodyIndex"></param>
        /// <returns></returns>
        public virtual async IAsyncEnumerable<IRow> GetBodyAsync(int bodyIndex = 1, bool removeQuotes = true)
        {
            if (_iterator.CurrentRow != bodyIndex)
            {
                if(!await _iterator
                    .JumpToAsync(bodyIndex)
                    .ConfigureAwait(false))

                        throw new CsvParserException($"can not set csv iterator to specific row = {bodyIndex}");
            }
                
            while (await _iterator
                .ReadLineAsync()
                .ConfigureAwait(false))
            {
                var columns = _rowSplitter.SplitRow<CsvColumn>(_iterator.GetValue(), _separator, removeQuotes);

                // empty row possibility
                if (!columns.Any()) continue;

                yield return new CsvRow(_iterator.CurrentRow, columns);

            }
        }

        public void Dispose()
        {
            _iterator.Dispose();
        }
    }
}
