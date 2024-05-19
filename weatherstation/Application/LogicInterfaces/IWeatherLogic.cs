using weatherstation.Domain.DTOs;

namespace weatherstation.Application.LogicInterfaces
{
    public interface IWeatherLogic
    {
        Task<CurrentWeatherDto> InsertWeatherData(dynamic data);
        Task<List<CurrentWeatherDto>> GetCurrentWeather();
        Task<List<CurrentWeatherDto>> GetAllWeather();
        Task<CurrentWeatherDto?> GetInstantWeather();
        Task<dynamic> GetStatistics(string interval);
    }
}
