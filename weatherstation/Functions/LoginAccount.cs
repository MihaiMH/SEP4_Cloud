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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData)
        {


            try
            {
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                var userData = JsonConvert.DeserializeObject<dynamic>(requestBody);
                Console.WriteLine(userData);
                var token = await AccountLogic.LoginAccount(userData);


                // Return JWT token
                return new OkObjectResult(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
