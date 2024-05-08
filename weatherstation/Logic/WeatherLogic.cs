using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Domain.DTOs;

namespace weatherstation.Logic
{
    internal static class WeatherLogic
    {
        public static CurrentWeatherDto GetCurrentWeather()
        {
            // LOGICA IN CARE SE IA DIN BAZA DE DATE

            CurrentWeatherDto currentWeather = new CurrentWeatherDto()
            {
                Location = "Horsens",
                CurrentTemp = 25.3,
                WeatherState = "Sunny",
                TimeChecked = DateTime.Now,
                Humidity = 5.3
            };

            return currentWeather;
        }
    }
}
