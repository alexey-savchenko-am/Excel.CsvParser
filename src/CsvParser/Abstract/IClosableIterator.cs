namespace CsvParser.Abstract
{
    using System.Threading.Tasks;

    public interface IClosableIterator
    {
        Task<bool> StartAsync();
        Task<bool> StopAsync();
    }
}
