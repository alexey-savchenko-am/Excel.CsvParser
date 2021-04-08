namespace CsvParser.Abstract
{
    using CsvParser.Abstract.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    public abstract class CsvParser<TModel>
         : IDisposable
         where TModel : ICsvModel, new()
    {
        protected readonly CsvDataProvider _dataProvider;
        public ICollection<TModel> Result { get; protected set; }

        public CsvParser(CsvDataProvider dataProvider)
        {
            Contract.Assert(dataProvider != null, "data provider is null");
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Process csv data source
        /// Parser extracts header of csv data source and initialize csvConverter
        /// </summary>
        /// <param name="csvConverter"></param>
        /// <param name="removeQuotes"></param>
        /// <param name="cleanUpResources"></param>
        /// <returns></returns>
		public async Task<bool> ProcessAsync(
            ICsvRowConvertionStrategy<TModel> csvConverter,
            bool removeQuotes = true,
            bool cleanUpResources = true)
		{
            if (_dataProvider == null)
                throw new CsvParserException("data Provider is not set");

            var iterator = _dataProvider.GetIterator();

            try
            {
                if (await iterator.StartAsync().ConfigureAwait(false))
                {
                    var header = await _dataProvider
                        .GetHeaderAsync(removeQuotes)
                        .ConfigureAwait(false);
                    
                    // initialize csv converter by header row if it has not been initialized before
                    var isInitialized = csvConverter.IsInitialized || csvConverter.Initialize(header);

                    if (!isInitialized)
                        throw new CsvParserException("can not initialize csv converter");

                    await ProcessBodyAsync(csvConverter).ConfigureAwait(false);
                }

            }
            finally
            {
                await iterator.StopAsync().ConfigureAwait(false);

                if (cleanUpResources)
                   this.Dispose();
            }

            return true;
		}

        /// <summary>
        /// Override to provide custom logic of body processing
        /// </summary>
        /// <returns></returns>
        protected virtual async Task ProcessBodyAsync(ICsvRowConvertionStrategy<TModel> csvConverter)
        {
            Result = new List<TModel>();

            await foreach(var bodyRow in _dataProvider.GetBodyAsync().ConfigureAwait(false))
            {
                var model = csvConverter.Convert(bodyRow.Columns);
                Result.Add(model);
            }
        }

        
        public void Dispose()
        {
            _dataProvider.Dispose();
        }
    }
}
