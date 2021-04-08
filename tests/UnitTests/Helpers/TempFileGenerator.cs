namespace UnitTests.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using UnitTests.Builders;

    public class TempFileGenerator
    {
        public string FilePath { get; set; }
        public string FileContent { get; set; }

        public bool FileCreated { get; set; }

        public async Task<int> GenerateAsync(char separator, int rowCount)
        {
            FileContent = CsvTextBuilder.BuildTyped(
                separator: separator,
                rowCount: rowCount,
                (typeof(string), "name"),
                (typeof(string), "surname"),
                (typeof(int), "age"),
                (typeof(DateTime), "lastActiveDate")
            );

            FilePath = $"tmp{DateTime.Now.Millisecond}.csv";

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            return await WriteTextAsync(FilePath, FileContent);
        }

        async Task<int> WriteTextAsync(string filePath, string text)
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
                FileCreated = true;
            }

            return encodedText.Length;
        }

    }
}
