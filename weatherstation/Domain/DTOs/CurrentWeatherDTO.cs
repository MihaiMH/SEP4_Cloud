namespace weatherstation.Domain.DTOs
{
    public class CurrentWeatherDto
    {
        public int Id { get; set; }
        public string WeatherState { get; set; }
        public double Temperature { get; set; }
        public double Light { get; set; }
        public double Humidity { get; set; }
        public DateTime Time { get; set; }

        public CurrentWeatherDto() { }

        public CurrentWeatherDto(string weatherState, double temperature, double light, double humidity, DateTime dateTime)
        {
            WeatherState = weatherState;
            Temperature = temperature;
            Light = light;
            Humidity = humidity;
            Time = dateTime;
        }
    }
}
