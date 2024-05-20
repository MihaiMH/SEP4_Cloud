using Microsoft.Azure.Functions.Worker.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography;
using weatherstation.Domain.Model;

namespace weatherstation.Utils
{
    public class Token
    {
        private static readonly string _validIssuer = Environment.GetEnvironmentVariable("JwtSettings:ValidIssuer", EnvironmentVariableTarget.Process);
        private static readonly string _validAudience = Environment.GetEnvironmentVariable("JwtSettings:ValidAudience", EnvironmentVariableTarget.Process);
        private static readonly string _signingKey = Environment.GetEnvironmentVariable("JwtSettings:SigningKey", EnvironmentVariableTarget.Process);
        private static readonly string _publicKey = Environment.GetEnvironmentVariable("JwtSettings:PublicKey", EnvironmentVariableTarget.Process);

        public Token()
        {

        }

        public Dictionary<string, string> Decode(string token)
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

        public string Extract(HttpRequestData httpRequest)
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

        public bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _validIssuer,
                ValidateAudience = true,
                ValidAudience = _validAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // This can be adjusted based on your requirements
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Issuer = _validIssuer,
                Audience = _validAudience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static RSA ReadPublicKey(string publicKeyBase64)
        {
            byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
            RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            return rsa;
        }
    }
}
