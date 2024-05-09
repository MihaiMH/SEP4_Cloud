using System;

public class IWeatherLogic
{
	public IWeatherLogic()
	{
		public Task<IEnumerable<WeatherData>> GetAsync(SearchParameters searchParameters);
	}
}
