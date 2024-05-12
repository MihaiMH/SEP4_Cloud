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
            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));

            string? query = Environment.GetEnvironmentVariable("SQLCON1Q1", EnvironmentVariableTarget.Process);

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
