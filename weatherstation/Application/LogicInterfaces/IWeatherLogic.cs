using System;

public interface IWeatherLogic
{
		public Task<IEnumerable<WeatherData>> GetAsync();
}
