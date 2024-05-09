using System;
using System.Collections;
using System.Threading.Tasks;

public class IWeatherDao
{
	public IWeatherDao()
	{
		public Task<IEnumerable<WeatherData>> GetAsync(SearchParameters searchParameters);
	}
}
