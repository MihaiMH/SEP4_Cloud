using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using weatherstation.Application.Logic;

namespace weatherstation.Functions
{
    internal class UpdateAccount
    {
        private readonly ILogger<UpdateAccount> _logger;

        public UpdateAccount(ILogger<UpdateAccount> logger)
        {
            _logger = logger;
        }

        [Function("UpdateAccount")]
        public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData reqData,
    FunctionContext executionContext)
        {
            try
            {
                string requestBody = await new StreamReader(reqData.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                // Update the account and get the new JWT token
                string token = await AccountLogic.UpdateAccount(data);

                // Return the JWT token as response
                return new OkObjectResult(new { token });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error updating account: {Message}", ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the account.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
