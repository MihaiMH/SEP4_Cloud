using Newtonsoft.Json;
using weatherstation.Domain.DTOs;
using weatherstation.Domain.Model;
using weatherstation.Utils;
using weatherstation.Application.LogicInterfaces;

namespace weatherstation.Application.Logic
{
    public class RecommendationLogic : IRecommendationLogic
    {
        private IWeatherLogic weatherLogic;
        private readonly IDBManager dbManager;

        public RecommendationLogic(IWeatherLogic weatherLogic, IDBManager dbManager)
        {
            this.weatherLogic = weatherLogic;
            this.dbManager = dbManager;
        }

        public async Task<string> GetRecommendation(dynamic data, Dictionary<string, string> token)
        {
            List<CurrentWeatherDto> weather = await weatherLogic.GetCurrentWeather();

            if (weather.Count == 0)
            {
                return "No weather data, can't generate a recommendation";
            }
            CurrentWeatherDto currentWeather = weather[0];

            string pref = "No preferences";

            if (token != null)
            {
                string sqlCon = Environment.GetEnvironmentVariable("SQLCON1Q4", EnvironmentVariableTarget.Process);

                sqlCon = sqlCon.Replace("[ID]", token["unique_name"]);

                List<Preference> preference = await dbManager.ExecuteQuery(sqlCon,
                   async (reader) => await Task.FromResult(new Preference
                   {
                       Text = reader.GetString("Preferences")
                   }));

                if (preference.Count > 0 && !String.IsNullOrEmpty(preference[0].Text))
                {
                    pref = preference[0].Text;
                }
            }

            string apiUrl = Environment.GetEnvironmentVariable("apiUrl", EnvironmentVariableTarget.Process);
            string apiKey = Environment.GetEnvironmentVariable("AIKEY", EnvironmentVariableTarget.Process);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                string query = Environment.GetEnvironmentVariable("AIQUERY", EnvironmentVariableTarget.Process);
                query = query.Replace("[WeatherState]", currentWeather.WeatherState.ToString())
                             .Replace("[Temperature]", currentWeather.Temperature.ToString())
                             .Replace("[Light]", currentWeather.Light.ToString())
                             .Replace("[Humidity]", currentWeather.Humidity.ToString())
                             .Replace("[PREFERENCES]", pref);

                var requestData = new
                {
                    model = "gpt-3.5-turbo",  // Set the model to gpt-3.5-turbo
                    messages = new[]
                    {
                        new { role = "user", content = query }
                    },
                    max_tokens = 75  // Set the maximum number of tokens to generate
                };
                var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                string result = await response.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);
                string msg = obj?.choices[0]?.message?.content;
                if (msg == null)
                {
                    msg = "An error occurred and the recommendation could not be generated";
                }

                return msg;
            }
        }
    }
}
