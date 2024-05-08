namespace Domain.Models;

public class Weather
{
    public int Id { get; set; }
    public double Humidity { get; set; }
    public double Temperature { get; set; }
    public string Light { get; set; }
    public DateTime DateTime { get; set; }
    
    public Weather() {}
    
    public Weather(int id, double humidity, double temperature, string light, DateTime dateTime)
    {
        Id = id;
        Humidity = humidity;
        Temperature = temperature;
        Light = light;
        DateTime = dateTime;
    }
}