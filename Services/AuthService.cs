
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Okada.Services
{
    public class AuthService
    {
        private readonly Dictionary<string, string> _verificationStore = new();
        private readonly string _jwtSecret = Guid.NewGuid().ToString();

        public async Task<bool> SendVerificationCodeAsync(string phoneNumber)
        {
            var code = new Random().Next(100000, 999999).ToString();
            _verificationStore[phoneNumber] = code;
            Console.WriteLine($"Sending code {code} to {phoneNumber}");
            return await Task.FromResult(true);
        }

        private string GenerateJWT(string phoneNumber)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Okada Go",
                audience: "Okada_Users",
                claims: new[] { new Claim(ClaimTypes.MobilePhone, phoneNumber) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // New public method for generating token directly
        public async Task<string> GenerateJwtTokenAsync(string phoneNumber)
        {
            return await Task.FromResult(GenerateJWT(phoneNumber));
        }

        // Now returns token instead of just true
        public async Task<string?> ValidateCodeAsync(string phoneNumber, string inputCode)
        {
            if (_verificationStore.TryGetValue(phoneNumber, out var realCode) && inputCode == realCode)
            {
                var token = GenerateJWT(phoneNumber);
                return await Task.FromResult(token);
            }
            return null;
        }
    }
}