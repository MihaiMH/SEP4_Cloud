namespace Domain.DTOs;

public class CurrentWeatherDto
{
    public string Location { get; }
    public double CurrentTemperature { get; }
    public string WeatherState { get; }
    public DateTime TimeChecked { get; }
    public double Humidity { get; }
    public string Light { get; }

    public CurrentWeatherDto(string location, double currentTemperature, string currentWeather, DateTime timeChecked,
        double humidity, string light)
    {
        Location = location;
        CurrentTemperature = currentTemperature;
        WeatherState = currentWeather;
        TimeChecked = timeChecked;
        Humidity = humidity;
        Light = light;
    }
}