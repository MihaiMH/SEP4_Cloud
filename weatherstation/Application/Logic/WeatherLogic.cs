using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Domain.DTOs;
using weatherstation.Utils;

namespace weatherstation.Logic
{
    internal class WeatherLogic
    {
        public WeatherLogic() {}

        public static void InsertWeatherData(dynamic data)
        {
            double temperature = data["temperature"];
            double humidity = data["humidity"];
            int light = data["light"];
            string queryTemplate = Environment.GetEnvironmentVariable("SQLCON1Q2", EnvironmentVariableTarget.Process);

            string query = queryTemplate
                .Replace("[VAR_TEMPERATURE]", temperature.ToString())
                .Replace("[VAR_LIGHT]", light.ToString())
                .Replace("[VAR_HUMIDITY]", humidity.ToString())
                .Replace("[VAR_DATETIME]", "'" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            Console.WriteLine(query);
            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            db.InsertData(query);
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
    }
}
