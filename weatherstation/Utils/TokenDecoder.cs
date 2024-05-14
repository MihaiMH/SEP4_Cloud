using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Utils
{
    internal class TokenDecoder
    {

        public  Dictionary<string, string> Decode(string token)
        {
            var TokenInfo = new Dictionary<string, string>();

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claims = jwtSecurityToken.Claims.ToList();

            foreach (var claim in claims)
            {
                try
                {
                    TokenInfo.Add(claim.Type, claim.Value);
                }
                catch (Exception ex)
                {
                    // Handle exception or log it
                }
            }

            return TokenInfo;
        }

        public  string Extract(HttpRequestData httpRequest)
        {
            if (httpRequest.Headers.TryGetValues("Authorization", out var authorizationValues))
            {
                var authorizationHeader = authorizationValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    return authorizationHeader.Replace("Bearer ", "");
                }
            }
            return null;
        }
        public string DictionaryToString(Dictionary<string, string> dictionary)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                stringBuilder.AppendLine($"{kvp.Key}: {kvp.Value}");
            }

            return stringBuilder.ToString();
        }
    }
}
