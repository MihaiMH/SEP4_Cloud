using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Domain.DTOs;
using weatherstation.Utils;

namespace weatherstation.Application.Logic
{
    public class WeatherLogic
    {
        public WeatherLogic() {}

        public static async Task<CurrentWeatherDto> InsertWeatherData(dynamic data)
        {
            double temperature = data["temperature"];
            double humidity = data["humidity"];
            double light = data["light"];
            string queryTemplate = Environment.GetEnvironmentVariable("SQLCON1Q2", EnvironmentVariableTarget.Process);
            string lightString;

            if (light <= 26)
            {
                lightString = "Sunny";
            }
            else if (light > 26 && light < 60)
            {
                lightString = "Little Cloudy";
            }
            else if (light >= 60 && light <= 80)
            {
                lightString = "Cloudy";
            }
            else
            {
                lightString = "Night";
            }

            string time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            string query = queryTemplate
                .Replace("[VAR_WEATHERSTATE]", lightString)
                .Replace("[VAR_TEMPERATURE]", temperature.ToString())
                .Replace("[VAR_LIGHT]", light.ToString())
                .Replace("[VAR_HUMIDITY]", humidity.ToString())
                .Replace("[VAR_DATETIME]", time);

            CurrentWeatherDto dto = new CurrentWeatherDto
            {
                Id = -5,
                WeatherState = lightString,
                Temperature = temperature,
                Humidity = humidity,
                Light = light,
                Time = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None)
            };
            Console.WriteLine(query);
            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            await db.InsertData(query);

            return dto;
        }

        public static async Task<List<CurrentWeatherDto>> GetCurrentWeather()
        {
            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));

            string? query = Environment.GetEnvironmentVariable("SQLCON1Q5", EnvironmentVariableTarget.Process);

            List <CurrentWeatherDto> results = await db.ExecuteQuery(
                query,
                async (reader) => await Task.FromResult(new CurrentWeatherDto
                {
                    Id = reader.GetInt32("Id"),
                    WeatherState = reader.GetString("WeatherState"),
                    Temperature = reader.GetDouble("Temperature"),
                    Light = reader.GetDouble("Light"),
                    Humidity = reader.GetDouble("Humidity"),
                    Time = reader.GetDateTime("DateTime")
                }));
            
            return results;
        }

        public static async Task<List<CurrentWeatherDto>> GetAllWeather()
        {
            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));

            string? query = Environment.GetEnvironmentVariable("SQLCON1Q1", EnvironmentVariableTarget.Process);

            List<CurrentWeatherDto> results = await db.ExecuteQuery(
                query,
                async (reader) => await Task.FromResult(new CurrentWeatherDto
                {
                    Id = reader.GetInt32("Id"),
                    WeatherState = reader.GetString("WeatherState"),
                    Temperature = reader.GetDouble("Temperature"),
                    Light = reader.GetDouble("Light"),
                    Humidity = reader.GetDouble("Humidity"),
                    Time = reader.GetDateTime("DateTime")
                }));

            return results;
        }

        public static async Task<CurrentWeatherDto?> GetInstantWeather()
        {
            string? SOCKET_IP = Environment.GetEnvironmentVariable("SOCKET_IP", EnvironmentVariableTarget.Process);
            string? SOCKET_PORT = Environment.GetEnvironmentVariable("SOCKET_PORT", EnvironmentVariableTarget.Process);
            string? MESSAGE2IOT = Environment.GetEnvironmentVariable("MESSAGE2IOT", EnvironmentVariableTarget.Process);
            SocketManager socket = new SocketManager(SOCKET_IP, Int32.Parse(SOCKET_PORT));
            await socket.SendMessageAndWaitForResponseAsync(MESSAGE2IOT);
            List<CurrentWeatherDto> list = await GetCurrentWeather();
            /*CurrentWeatherDto? dto = JsonConvert.DeserializeObject<CurrentWeatherDto>(response);*/
            return list[0];
        }

        public static async Task<dynamic> GetStatistics(string interval)
        {
            
            string? q1 = Environment.GetEnvironmentVariable("STATSQ1", EnvironmentVariableTarget.Process);
            string? q2 = Environment.GetEnvironmentVariable("STATSQ2", EnvironmentVariableTarget.Process);
            string? q3 = Environment.GetEnvironmentVariable("STATSQ3", EnvironmentVariableTarget.Process);
            string? q4 = Environment.GetEnvironmentVariable("STATSQ4", EnvironmentVariableTarget.Process);

            q1 = q1.Replace("[INTERVAL]", interval);
            q2 = q2.Replace("[INTERVAL]", interval);
            q3 = q3.Replace("[INTERVAL]", interval);
            q4 = q4.Replace("[INTERVAL]", interval);


            var summary =  ExecuteJsonQuery(q1);
            var temperatureGraph =  ExecuteJsonQuery(q2);
            var averageHumidity =  ExecuteJsonQuery(q3);
            var weatherStateSummary =  ExecuteJsonQuery(q4);

            // Await all tasks to complete
            await Task.WhenAll(summary, temperatureGraph, averageHumidity, weatherStateSummary);

            // Combine results
            var allData = new
            {
                summary = Newtonsoft.Json.JsonConvert.DeserializeObject(summary.Result),
                temperatureGraph = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(temperatureGraph.Result),
                averageHumidity = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(averageHumidity.Result),
                weatherStateSummary = Newtonsoft.Json.JsonConvert.DeserializeObject(weatherStateSummary.Result)
            };

            // Convert the combined data object back to JSON string

            return allData;

        }

        private static async Task<string> ExecuteJsonQuery(string query)
        {

            DBManager db = new DBManager(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
            var results = await db.ExecuteQuery(query, async reader =>
            {
                string jsonData = await reader.GetFieldValueAsync<string>(0); // Assuming JSON is in the first column
                return new JsonResultData { JsonData = jsonData };
            });

            return results.FirstOrDefault()?.JsonData ?? "{}"; // Return JSON or empty JSON object
        }
    }
}
