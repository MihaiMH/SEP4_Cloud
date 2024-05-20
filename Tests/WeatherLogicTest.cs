using Moq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [Fact]
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
        }
    }
}
