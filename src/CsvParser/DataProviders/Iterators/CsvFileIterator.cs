namespace CsvParser.DataProviders.Iterators
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class CsvFileIterator
        : CsvStreamIterator
    {
        private readonly string _filePath;
   
        public string FilePath { get => _filePath; }

        public CsvFileIterator(string filePath)
            : this(filePath, Encoding.UTF8)
        {}

        public CsvFileIterator(string filePath, Encoding encoding)
        {
            if (filePath == null || encoding == null)
                throw new ArgumentNullException(filePath == null ? nameof(filePath) : nameof(encoding));

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            
            _filePath = filePath;
            _encoding = encoding;
        }

        public override async Task<bool> StartAsync()
        {
            if (IsStarted)
                throw new CsvParserException("iterator is already started");

            var stream =
               new FileStream(
                   _filePath,
                   FileMode.Open,
                   FileAccess.Read,
                   FileShare.Read,
                   bufferSize: BufferSize,
                   useAsync: true
               );

            IsStarted = true;

            return Initialize(stream, _encoding);
        }

        /// <summary>
        /// Close file stream physically, so we can reopen it with StartAsync method
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> StopAsync()
        {
            IsStarted = false;
            return DisposeInternal();
        }

    }
}
