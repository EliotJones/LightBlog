using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightBlog.Models
{
    public class UploadAuthentication : IUploadAuthentication
    {
        private const int SaltSize = 128 / 8;
        private const int Iterations = 1000;
        private const int BytesRequested = 256 / 8;
        private const int Minutes = 3;

        private readonly IOptions<UploadOptions> options;
        private readonly ILogger<UploadAuthentication> logger;

        public UploadAuthentication (IOptions<UploadOptions> options, 
            ILogger<UploadAuthentication> logger)
        {   
            this.options = options;
            this.logger = logger;
        }

        public string AuthenticateStepOne(string password)
        {
            if (options.Value.UploadLoginPassword != password)
            {
                return string.Empty;
            }

            var tempPass = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            var salt = new byte[SaltSize];

            var random = RandomNumberGenerator.Create();

            random.GetBytes(salt);

            byte[] subkey = KeyDerivation.Pbkdf2(tempPass, salt, 
                KeyDerivationPrf.HMACSHA256,
                Iterations,
                BytesRequested);

            var fileContent = $"{Convert.ToBase64String(salt)}\r\n{tempPass}\r\n"
                + $"{DateTime.UtcNow.AddMinutes(Minutes)}";

            File.WriteAllText("verify.txt", fileContent);

            return Convert.ToBase64String(subkey);
        }

        public bool AuthenticateUploadAttempt(string token, string password)
        {
            if (!HasValidToken(token))
            {
                return false;
            }

            return password == options.Value.UploadPassword;
        }

        public bool HasValidToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || !File.Exists("verify.txt"))
            {
                return false;
            }

            var file = File.ReadAllLines("verify.txt");

            try
            {
                var salt = file[0];
                var tempPass = file[1];
                var expiryDate = DateTime.Parse(file[2]);  

                if (DateTime.UtcNow > expiryDate)
                {
                    logger.LogInformation("Token expired");

                    return false;
                }

                var saltBytes = Convert.FromBase64String(salt);
                byte[] subkey = KeyDerivation.Pbkdf2(tempPass, saltBytes, 
                    KeyDerivationPrf.HMACSHA256,
                    Iterations,
                    BytesRequested);

                var subkeyString = Convert.ToBase64String(subkey); 

                if (subkeyString != token)
                {
                    logger.LogInformation("Token did not match");
                    return false;
                }

                return true;              
            }
            catch (System.Exception ex)
            {
                logger.LogInformation("Could not read the verify file. " + ex.Message);
            }

            return false;
        }
    }

    public interface IUploadAuthentication
    {
        string AuthenticateStepOne(string password);

        bool AuthenticateUploadAttempt(string token, string password);

        bool HasValidToken(string token);
    }
}