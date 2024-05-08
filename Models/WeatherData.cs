public class WeatherData
{
	public double CurrentTemperature { get; set; }
	public double WindSpeed { get; set; }
	public bool IsRaining { get; set; }

	public WeatherData(double currentTemperature, double windSpeed, bool isRaining)
	{
		CurrentTemperature = currentTemperature;
		WindSpeed = windSpeed;
		IsRaining = isRaining
	}

}