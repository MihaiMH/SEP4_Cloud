using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Application.DaoInterfaces
{
    public interface IWeatherDao
    {
        public Task<IEnumerable<WeatherData>> GetAsync();
    }
}
