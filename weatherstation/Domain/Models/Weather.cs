namespace Domain.Models;

public class Weather
{
    public int Id { get; set; }
    public double Humidity { get; set; }
    public string Location { get; set; }
    public double Temperature { get; set; }
    public string Light { get; set; }
    public string WeatherState { get; set; }
    public DateTime DateTime { get; set; }
    
    public Weather() {}
    
    public Weather(int id, double humidity, double temperature, string light, DateTime dateTime, string location, string weatherState)
    {
        Id = id;
        Humidity = humidity;
        Temperature = temperature;
        Light = light;
        DateTime = dateTime;
        Location = location;
        WeatherState = weatherState;
    }
}