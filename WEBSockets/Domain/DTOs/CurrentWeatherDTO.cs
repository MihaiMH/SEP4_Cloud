using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEBSockets.Domain.DTOs
{
    public class CurrentWeatherDto
    {
        public int Id { get; set; }
        public string WeatherState { get; set; }
        public double Temperature { get; set; }
        public string Light { get; set; }
        public double Humidity { get; set; }
        public DateTime Time { get; set; }

        public CurrentWeatherDto() { }

        public CurrentWeatherDto(string weatherState, double temperature, string light, double humidity, DateTime dateTime)
        {
            WeatherState = weatherState;
            Temperature = temperature;
            Light = light;
            Humidity = humidity;
            Time = dateTime;
        }
    }
}
