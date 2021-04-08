namespace CsvParser.Abstract
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class CsvIterator
        : IDisposable, IClosableIterator
    {
        public bool IsStarted { get; set; }

        protected const int BufferSize = 4096;
        protected const int MinBufferSize = 128;
        
        protected bool _disposed = false;

        protected int _row = 0;
        protected char[] _charBuffer;
        protected byte[] _byteBuffer;
        protected int _charPos;
        protected int _charLen;
        protected int _bytePos;
        protected int _byteLen;

        protected StringBuilder _value = null;

        /// <summary>
        /// Currently processing row
        /// </summary>
        public int CurrentRow { get => _row; }

        /// <summary>
        /// Force iterator jump to specific row
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> JumpToAsync(int rowIndex);


        /// <summary>
        /// Extracts symbol's string from the current row
        /// </summary>
        /// <returns></returns>
        public virtual string GetValue()
            => _value.ToString();


        /// <summary>
        /// Force iterator jump to begining of source
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> ResetAsync();


        /// <summary>
        /// Read a single line from specified source
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> ReadLineAsync();


        /// <summary>
        /// Read until the end of specified source
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> ReadAllAsync();

        public virtual async Task<bool> StartAsync()
        {
            if (IsStarted)
                throw new CsvParserException("csv iterator is already started");

            return IsStarted = true;
        }

        public virtual async Task<bool> StopAsync()
        {
            return IsStarted = false;
        }

        public abstract void Dispose();
    }
}
