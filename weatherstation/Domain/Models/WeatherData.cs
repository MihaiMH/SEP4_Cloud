using System;

public class WeatherData
{
	public int Id { get; set; }
	public string WeatherState { get; set; }
	public double Temperature { get; set; }
	public string Light { get; set; }
	public double Humidity { get; set; }
	public DateTime DateTime { get; set; }

	public WeatherData() { }

	public WeatherData(string weatherState, double temperature, string light, double humidity, DateTime dateTime) {
		WeatherState = weatherState;
		Temperature = temperature;
		Light = light;
		Humidity = humidity;
		DateTime = dateTime;
	}

}