using Application.DTO.GoogleDTO;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public class GoogleService(IConfiguration _configuration) : IGoogleService
    {
        public async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["Google:ClientId"] }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }
    }
}
