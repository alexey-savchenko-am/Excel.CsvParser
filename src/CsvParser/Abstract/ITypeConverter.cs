using System.Threading.Tasks;

namespace CsvParser.Abstract
{
	public interface ITypeConverter
    {
		object Convert(object value);
	}

    public interface IAsyncTypeConverter<TFrom, TTo>
	{
		Task<TTo> ConvertAsync(TFrom value);
	}

	public interface ITypeConverter<TFrom, TTo>
	{
		TTo Convert(TFrom value);
	}
}
