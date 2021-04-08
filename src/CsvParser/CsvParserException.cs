namespace CsvParser
{
    using System;

    public class CsvParserException
        : Exception
    {
        public CsvParserException(string message) 
            : base(message)
        {
        }

        public CsvParserException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
