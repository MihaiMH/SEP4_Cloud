namespace Domain.DTOs;

public class BasicForecastDto
{   
    public DateTime Time { get; }
    public double Temperature { get; }
    public string WeatherState { get; }

    public BasicForecastDto(DateTime time, double temperature, string weatherForecast)
    {
        Time = time;
        Temperature = temperature;
        WeatherState = weatherForecast;
    }
}