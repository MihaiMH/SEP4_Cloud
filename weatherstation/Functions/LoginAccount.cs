using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Application.Logic;
using Microsoft.Extensions.Logging;

namespace weatherstation.Functions
{
    internal class LoginAccount
    {
        private readonly ILogger<LoginAccount> _logger;

        public LoginAccount(ILogger<LoginAccount> logger) {
            _logger = logger;
        }

        [Function("LoginAccount")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData)
        {
            var res = reqData.CreateResponse();

            try
            {
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                var userData = JsonConvert.DeserializeObject<dynamic>(requestBody);
                var token = await AccountLogic.LoginAccount(userData);

                res.StatusCode = System.Net.HttpStatusCode.OK;
                var json = JsonConvert.SerializeObject(new { token = token });
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
                return res;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error logging in: {Message}", ex.Message);
                res.StatusCode = System.Net.HttpStatusCode.BadRequest;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = ex.Message })));
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                res.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                res.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { error = "An error occurred while logging in." })));
                return res;
            }
        }
    }
}
