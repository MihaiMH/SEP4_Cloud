using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using weatherstation.Application.Logic;
using weatherstation.Domain.Model;

namespace weatherstation.Functions
{
    public class RegisterAccount
    {
        private readonly ILogger<RegisterAccount> _logger;
        private readonly AccountLogic _accountLogic;

        public RegisterAccount(ILogger<RegisterAccount> logger)
        {
            _logger = logger;
            _accountLogic = new AccountLogic(Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process));
        }

        [Function("RegisterAccount")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData)
        {
            try
            {
                // Citim ce trimit ciuspanii din front
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                var user = JsonConvert.DeserializeObject<User>(requestBody);

                // Register the account
                await _accountLogic.RegisterAccount(user);

                // Scrim ca tot e norm
                _logger.LogInformation("User account registered successfully.");

                return new OkResult();
            }
            catch (Exception ex)
            {
                // Scrim ca nu tot norm
                _logger.LogError(ex, "Error registering account");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
