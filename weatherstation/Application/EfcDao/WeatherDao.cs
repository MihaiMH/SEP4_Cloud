using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Application.DaoInterfaces;
using 


namespace weatherstation.Application.Dao
{
    internal class WeatherDao : IWeatherDao
    {
       private 
        public Task<IEnumerable<WeatherData>> GetAsync()
        {
            throw new NotImplementedException();
        }
    }
}
