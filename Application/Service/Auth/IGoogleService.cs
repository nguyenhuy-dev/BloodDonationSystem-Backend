using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public interface IGoogleService
    {
        Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token);
    }
}
