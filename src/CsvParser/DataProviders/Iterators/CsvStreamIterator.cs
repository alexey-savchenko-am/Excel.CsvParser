namespace CsvParser.DataProviders.Iterators
{
    using CsvParser.Abstract;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class CsvStreamIterator
        : CsvIterator
    {
        protected Stream _stream;
        protected Encoding _encoding;
        protected Decoder _decoder;


        protected CsvStreamIterator() { }

        public CsvStreamIterator(Stream stream)
            : this(stream, Encoding.UTF8)
        { }

        public CsvStreamIterator(Stream stream, Encoding encoding)
        {
            if (stream == null || encoding == null)
                throw new ArgumentNullException(stream == null ? nameof(stream) : nameof(encoding));

            if (!stream.CanRead)
                throw new InvalidOperationException("can not read from stream");

            Initialize(stream, encoding);
        }

        public override async Task<bool> ResetAsync()
        {
            if (_stream.Position == 0)
                return true;

            // put the pointer to begining
            if (_stream != null && _stream.CanSeek)
            {
                _stream.Seek(0, SeekOrigin.Begin);
                ResetVariables();
                return _stream.Position == 0;
            }

            throw new InvalidOperationException("can not reset iterator");
        }


        /// <summary>
        /// Jump to specific row of csv file
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public override async Task<bool> JumpToAsync(int rowIndex)
        {
            if(!await ResetAsync().ConfigureAwait(false))
                return false;

            var iteration = 1;
            while (await ReadLineAsync().ConfigureAwait(false))
            {
                _row++;
                if (iteration++ == rowIndex)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Reads all data from specific file
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> ReadAllAsync()
        {
            // if there are no more data in file
            if (_charPos == _charLen &&
                    (await FillBufferAsync().ConfigureAwait(false)) == 0)
                return false;

            _value = new StringBuilder(_charLen - _charPos);

            do
            {
                _value.Append(_charBuffer, _charPos, _charLen - _charPos);
                _charPos = _charLen;

            } while (await FillBufferAsync().ConfigureAwait(false) > 0);


            return _value.Length > 0;
        }

        /// <summary>
        /// Reads file from current position until the end of line symbol
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> ReadLineAsync()
        {
            // if there are no more data in file
            if (_charPos == _charLen &&
                    (await FillBufferAsync().ConfigureAwait(false)) == 0)
                return false;

            _value = new StringBuilder();

            do
            {
                var i = _charPos;

                while (i < _charLen)
                {
                    var currentSymbol = _charBuffer[i];

                    if (currentSymbol == '\r' || currentSymbol == '\n')
                    {
                        _value.Append(_charBuffer, _charPos, i - _charPos);
                        _charPos = i + 1;
                        
                        // increment current row variable
                        _row++;

                        //check wheather next symbol is \n or not
                        if (currentSymbol == '\r'
                                && (_charPos < _charLen || (await FillBufferAsync().ConfigureAwait(false)) > 0))
                        {
                            if (_charBuffer[_charPos] == '\n')
                                _charPos++;
                        }
                        return true;
                    }

                    i++;
                }

                i = _charLen - _charPos;
                _value.Append(_charBuffer, _charPos, i);
            }
            while (await FillBufferAsync().ConfigureAwait(false) > 0);

            return true;
        }

        private async Task<int> FillBufferAsync()
        {
            _charPos = _charLen = 0;
            _byteLen = _bytePos = 0;

            do
            {
                _byteLen = await _stream.ReadAsync(_byteBuffer, 0, _byteBuffer.Length).ConfigureAwait(false);

                if (_byteLen == 0) // EOF
                    return _charLen;

                _charLen += _decoder.GetChars(_byteBuffer, 0, _byteLen, _charBuffer, _charLen);

            } while (_charLen == 0);


            return _charLen;
        }

        protected bool Initialize(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
            _decoder = encoding.GetDecoder();

            if (_stream.Position > 0)
                _stream.Seek(0, SeekOrigin.Begin);

            ResetVariables();

            return true;
        }

        private void ResetVariables()
        {
            _disposed = false;
            _value = null;
            _charBuffer = new char[BufferSize];
            _byteBuffer = new byte[BufferSize];
            _charPos = 0;
            _bytePos = 0;
            _charLen = 0;
            _byteLen = 0;
            _row = 0;
        }


        public override void Dispose()
        {
            // do not let to dispose iterator twise
            if (_disposed) return;

            DisposeInternal();

            _disposed = true;
        }

        protected bool DisposeInternal()
        {
            try
            {
                if (_stream != null)
                    _stream.Close();
            }
            finally
            {
                if (_stream != null)
                {
                    _stream = null;
                    _encoding = null;
                    _decoder = null;
                    _charBuffer = null;
                    _byteBuffer = null;
                    _charPos = 0;
                    _bytePos = 0;
                    _charLen = 0;
                    _byteLen = 0;
                }
            }

            return _stream == null;
        }

        public override async Task<bool> StartAsync()
        {   
            return await base.StartAsync();
        }

        /// <summary>
        /// Do not close stream, just set pointer to begining of the stream
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> StopAsync()
        {
            if (_stream.Position > 0)
                _stream.Seek(0, SeekOrigin.Begin);

            ResetVariables();

            return await base.StopAsync();
        }


    }
}

