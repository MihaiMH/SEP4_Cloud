namespace weatherstation.DTOs;

public class BasicForecastDto
{
    public DateTime Time { get; }
    public double Temperature { get; }
    public string WeatherState { get; }
    
    public BasicForecastDto(){}

    public BasicForecastDto(DateTime time, double temperature, string weatherState)
    {
        Time = time;
        Temperature = temperature;
        WeatherState = weatherState;
    }
}