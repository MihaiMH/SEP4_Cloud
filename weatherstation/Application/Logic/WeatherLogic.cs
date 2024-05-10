using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Application.DaoInterfaces;
using weatherstation.Domain.DTOs;
using weatherstation.Utils;

namespace weatherstation.Logic
{
    internal class WeatherLogic
    {
        private readonly IWeatherDao weatherDao;

        public WeatherLogic(IWeatherDao weatherDao)
        {
            this.weatherDao = weatherDao;
        }

        public static List<CurrentWeatherDto> GetCurrentWeather()
        {
            DBManager db = new DBManager("Server=sql.freedb.tech;Port=3306;Database=freedb_weatherstation;Uid=freedb_cristi;Pwd=wx*kQ6Ez7gK#6Jg;");

            string? query = "SELECT * FROM WeatherData";

            List <CurrentWeatherDto> results = db.ExecuteQuery(
                query,
                reader => new CurrentWeatherDto
                {
                    Id = reader.GetInt32("Id"),
                    WeatherState = reader.GetString("WeatherState"),
                    Temperature = reader.GetDouble("Temperature"),
                    Light = reader.GetString("Light"),
                    Humidity = reader.GetDouble("Humidity"),
                    Time = reader.GetDateTime("DateTime")
                });
            
            return results;

        }

        public Task<IEnumerable<WeatherData>> GetWeatherAsync() { 
            return weatherDao.GetAsync();
        }
    }
}
