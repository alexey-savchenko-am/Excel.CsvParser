namespace UnitTests.CsvIterators
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using CsvParser.DataProviders.Iterators;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using UnitTests.Builders;
    using Xunit;

    public class FileIteratorTests
        : CleanableTest
    {
        private const char Separator = ',';
        private const string FilePath = "./tmp.csv";
        private const int RowCount = 100000;

        private readonly string _csvText;

        public FileIteratorTests()
        {
            // arrange

            _csvText = CsvTextBuilder.BuildTyped(
                separator: Separator, 
                rowCount: RowCount,
                (typeof(string), "name"),
                (typeof(string), "surname"),
                (typeof(int), "age"),
                (typeof(DateTime), "lastActiveDate")
            );

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            WriteTextAsync(FilePath, _csvText).Wait();
        }

        [Fact]
        [Benchmark]
        public async Task IteratorSuccessfullyReadAllDataFromPhysicalFile()
        {
            // act
            string result = null;
            using (var iterator = new CsvFileIterator(FilePath))
            {
                if(await iterator.ReadAllAsync())
                {
                    result = iterator.GetValue();
                }
            }

            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(_csvText.Length, result.Length);
        }


        [Fact]
        [Benchmark]
        public async Task IteratorSuccessfullyReadDataLineByLineFromPhysicalFile()
        {
            // act
            var result = new List<string>();
            int position = 0;
            using (var iterator = new CsvFileIterator(FilePath))
            {
                while (await iterator.ReadLineAsync())
                {
                    var line = iterator.GetValue();
                    result.Add(line);
                    position = iterator.CurrentRow;
                }
            }


            // assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(RowCount + 1, result.Count);
            Assert.Equal(result.Count, position);
        }




        async Task WriteTextAsync(string filePath, string text)
        {
  
            byte[] encodedText = Encoding.UTF8.GetBytes(text);

            using (var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        protected override void CleanUp()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
