using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Domain.DTOs
{
    public class CurrentWeatherDto
    {
        public string Location { get; set; }
        public double CurrentTemp { get; set; }
        public string WeatherState { get; set; }
        public DateTime TimeChecked { get; set; }
        public double Humidity { get; set; }

        public CurrentWeatherDto() { }

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
}
