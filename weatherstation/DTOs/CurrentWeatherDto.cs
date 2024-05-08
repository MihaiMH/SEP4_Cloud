namespace weatherstation.DTOs;

public class CurrentWeatherDto
{
    public string Location { get; }
    public double CurrentTemp { get; }
    public string WeatherState { get; }
    public DateTime TimeChecked { get; }
    public double Humidity { get; }

    public CurrentWeatherDto(){}
    
    public CurrentWeatherDto(string location, double currentTemp, string weatherState, DateTime timeChecked,
        double humidity)
    {
        Location = location;
        CurrentTemp = currentTemp;
        WeatherState = weatherState;
        TimeChecked = timeChecked;
        Humidity = humidity;
    }
}