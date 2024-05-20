using Moq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using weatherstation.Application.Logic;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Domain.DTOs;
using weatherstation.Utils;

namespace Tests
{
    public class WeatherLogicTest
    {
        private readonly Mock<IDBManager> _mockDbManager;
        private readonly Mock<ISocketManager> _mockSocketManager;
        private readonly IWeatherLogic _weatherLogic;

        public WeatherLogicTest()
        {
            _mockDbManager = new Mock<IDBManager>();
            _mockSocketManager = new Mock<ISocketManager>();
            _weatherLogic = new WeatherLogic(_mockDbManager.Object, _mockSocketManager.Object);
        }

        [Fact]
        public async Task InsertWeatherData_ShouldReturnCurrentWeatherDto()
        {
            // Arrange
            string jsonData = "{\"temperature\": 25, \"humidity\": 60, \"light\": 30}";
            dynamic data = JObject.Parse(jsonData);

            Environment.SetEnvironmentVariable("SQLCON1Q2", "INSERT INTO WeatherData (WeatherState, Temperature, Light, Humidity, DateTime) VALUES ('[VAR_WEATHERSTATE]', [VAR_TEMPERATURE], [VAR_LIGHT], [VAR_HUMIDITY], '[VAR_DATETIME]');");

            _mockDbManager.Setup(db => db.InsertData(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _weatherLogic.InsertWeatherData(data);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25, result.Temperature);
            Assert.Equal(60, result.Humidity);
            Assert.Equal(30, result.Light);
            Assert.Equal("Little Cloudy", result.WeatherState);
        }

        [Fact]
        public async Task GetCurrentWeather_ShouldReturnOneCurrentWeatherDtoInTheList()
        {
            // Arrange
            var weatherDataList = new List<CurrentWeatherDto>
            {
                new CurrentWeatherDto
                {
                    Id = 1,
                    WeatherState = "Sunny",
                    Temperature = 25,
                    Humidity = 60,
                    Light = 20,
                    Time = DateTime.UtcNow
                },
                new CurrentWeatherDto
                {
                    Id = 2,
                    WeatherState = "Sunny",
                    Temperature = 25,
                    Humidity = 60,
                    Light = 20,
                    Time = DateTime.UtcNow
                },
            };


            _mockDbManager.Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Func<MySqlDataReader, Task<CurrentWeatherDto>>>())).ReturnsAsync(weatherDataList);

            // Act
            var result = await _weatherLogic.GetCurrentWeather();
            var res = new List<CurrentWeatherDto> { result[0] };

            // Assert
            Assert.NotNull(res);
            Assert.Single(res);
            Assert.Equal(1, res[0].Id);
            Assert.Equal("Sunny", res[0].WeatherState);
            Assert.Equal(25, res[0].Temperature);
            Assert.Equal(60, res[0].Humidity);
            Assert.Equal(20, res[0].Light);
        }

        [Fact]
        public async Task GetAllWeather_ShouldReturnListOfCurrentWeatherDto()
        {
            // Arrange
            var weatherDataList = new List<CurrentWeatherDto>
            {
                new CurrentWeatherDto
                {
                    Id = 1,
                    WeatherState = "Sunny",
                    Temperature = 25,
                    Humidity = 60,
                    Light = 20,
                    Time = DateTime.UtcNow
                },
                new CurrentWeatherDto
                {
                    Id= 2,
                    WeatherState = "Night",
                    Temperature = 15,
                    Humidity = 24,
                    Light = 90,
                    Time = DateTime.UtcNow
                }
            };

            _mockDbManager.Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Func<MySqlDataReader, Task<CurrentWeatherDto>>>()))
                          .ReturnsAsync(weatherDataList);

            // Act
            var result = await _weatherLogic.GetAllWeather();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Sunny", result[0].WeatherState);
            Assert.Equal(2, result[1].Id);
            Assert.Equal("Night", result[1].WeatherState);
        }

        /*[Fact]
        public async Task GetInstantWeather_ShouldReturnCurrentWeatherDto()
        {
            // Arrange
            var weatherData = new CurrentWeatherDto
            {
                Id = 1,
                WeatherState = "Sunny",
                Temperature = 25,
                Humidity = 60,
                Light = 20,
                Time = DateTime.UtcNow
            };

            var weatherDataList = new List<CurrentWeatherDto>
            {
                weatherData
            };

            string jsonString = JsonConvert.SerializeObject(weatherData);

            _mockSocketManager.Setup(sm => sm.SendMessageAndWaitForResponseAsync(It.IsAny<string>())).Returns(Task.FromResult(jsonString));
            _mockDbManager.Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Func<MySqlDataReader, Task<CurrentWeatherDto>>>()))
                          .ReturnsAsync(weatherDataList);

            // Act
            var result = await _weatherLogic.GetInstantWeather();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(weatherData.Id, result.Id);
            Assert.Equal(weatherData.WeatherState, result.WeatherState);
            Assert.Equal(weatherData.Temperature, result.Temperature);
            Assert.Equal(weatherData.Humidity, result.Humidity);
            Assert.Equal(weatherData.Light, result.Light);
            Assert.Equal(weatherData.Time, result.Time);
        }*/

        /*[Fact]
        public async Task GetStatistics_ShouldReturnStatisticsData()
        {
            // Arrange
            string interval = "week";

            string q1 = "SELECT JSON_OBJECT('sunnyDays', COUNT(DISTINCT CASE WHEN WeatherState = 'Sunny' THEN DATE(DateTime) END), 'averageLight', CAST(AVG(Light) AS UNSIGNED), 'averageTemperature', CAST(AVG(Temperature) AS UNSIGNED), 'averageHumidity', CAST(AVG(Humidity) AS UNSIGNED)) AS summary FROM WeatherData WHERE DateTime >= NOW() - INTERVAL 1 [INTERVAL]";
            string q2 = "SELECT JSON_ARRAYAGG(JSON_OBJECT('date', dateFormatted, 'minTemperature', minTemperature, 'maxTemperature', maxTemperature)) AS temperatureGraph FROM (SELECT DATE(DateTime) AS dateFormatted, CAST(MIN(Temperature) AS UNSIGNED) AS minTemperature, CAST(MAX(Temperature) AS UNSIGNED) AS maxTemperature FROM WeatherData WHERE DateTime >= NOW() - INTERVAL 1 [INTERVAL] GROUP BY DATE(DateTime)) AS dailyTemps";
            string q3 = "SELECT JSON_ARRAYAGG(JSON_OBJECT('date', dateFormatted, 'humidity', avgHumidity)) AS averageHumidity FROM (SELECT DATE(DateTime) AS dateFormatted, CAST(AVG(Humidity) AS UNSIGNED) AS avgHumidity FROM WeatherData WHERE DateTime >= NOW() - INTERVAL 1 [INTERVAL] GROUP BY DATE(DateTime)) AS dailyHumidity";
            string q4 = "SELECT JSON_OBJECT('littleclody', SUM(WeatherState LIKE '%Little Cloudy%'), 'cloudy', SUM(WeatherState LIKE '%Cloudy%'), 'sunny', SUM(WeatherState LIKE '%Sunny%')) AS weatherStateSummary FROM (SELECT DISTINCT DATE(DateTime) AS Day, WeatherState FROM WeatherData WHERE DateTime >= NOW() - INTERVAL 1 [INTERVAL]) AS UniqueDays";

            q1 = q1.Replace("[INTERVAL]", interval);
            q2 = q2.Replace("[INTERVAL]", interval);
            q3 = q3.Replace("[INTERVAL]", interval);
            q4 = q4.Replace("[INTERVAL]", interval);

            string summaryJson = "{\"sunnyDays\": 4, \"averageLight\": 53, \"averageHumidity\": 32, \"averageTemperature\": 21}";
            string temperatureGraphJson = "[{\"date\": \"2024-05-14\", \"maxTemperature\": 26, \"minTemperature\": 26}, {\"date\": \"2024-05-15\", \"maxTemperature\": 30, \"minTemperature\": 27}, {\"date\": \"2024-05-16\", \"maxTemperature\": 100, \"minTemperature\": 1}, {\"date\": \"2024-05-17\", \"maxTemperature\": 24, \"minTemperature\": 3}, {\"date\": \"2024-05-18\", \"maxTemperature\": 25, \"minTemperature\": 24}, {\"date\": \"2024-05-19\", \"maxTemperature\": 30, \"minTemperature\": 3}]";
            string averageHumidityJson = "[{\"date\": \"2024-05-14\", \"humidity\": 33}, {\"date\": \"2024-05-15\", \"humidity\": 39}, {\"date\": \"2024-05-16\", \"humidity\": 24}, {\"date\": \"2024-05-17\", \"humidity\": 40}, {\"date\": \"2024-05-18\", \"humidity\": 47}, {\"date\": \"2024-05-19\", \"humidity\": 25}]";
            string weatherStateSummaryJson = "{\"sunny\": 4, \"cloudy\": 8, \"littleclody\": 4}";

            // Mock the ExecuteJsonQuery method to return the JSON string
            _mockDbManager.Setup(db => db.ExecuteQuery(It.Is<string>(q => q == q1), It.IsAny<Func<IDataReader, Task<JsonResultData>>>()))
            .ReturnsAsync(new List<JsonResultData> { new JsonResultData { JsonData = summaryJson } });

            _mockDbManager.Setup(db => db.ExecuteQuery(It.Is<string>(q => q == q2), It.IsAny<Func<IDataReader, Task<JsonResultData>>>()))
                .ReturnsAsync(new List<JsonResultData> { new JsonResultData { JsonData = temperatureGraphJson } });

            _mockDbManager.Setup(db => db.ExecuteQuery(It.Is<string>(q => q == q3), It.IsAny<Func<IDataReader, Task<JsonResultData>>>()))
                .ReturnsAsync(new List<JsonResultData> { new JsonResultData { JsonData = averageHumidityJson } });

            _mockDbManager.Setup(db => db.ExecuteQuery(It.Is<string>(q => q == q4), It.IsAny<Func<IDataReader, Task<JsonResultData>>>()))
                .ReturnsAsync(new List<JsonResultData> { new JsonResultData { JsonData = weatherStateSummaryJson } });

            // Act
            var result = await _weatherLogic.GetStatistics(interval);

            // Assert
            Assert.NotNull(result);

            var expectedSummary = JsonConvert.DeserializeObject<dynamic>(summaryJson);
            var expectedTemperatureGraph = JsonConvert.DeserializeObject<List<dynamic>>(temperatureGraphJson);
            var expectedAverageHumidity = JsonConvert.DeserializeObject<List<dynamic>>(averageHumidityJson);
            var expectedWeatherStateSummary = JsonConvert.DeserializeObject<dynamic>(weatherStateSummaryJson);

            Assert.Equal(expectedSummary, result.summary);
            Assert.Equal(expectedTemperatureGraph, result.temperatureGraph);
            Assert.Equal(expectedAverageHumidity, result.averageHumidity);
            Assert.Equal(expectedWeatherStateSummary, result.weatherStateSummary);
        }*/
    }
}
