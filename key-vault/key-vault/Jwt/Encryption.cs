using Microsoft.IdentityModel.Tokens;
using System.Text;
using key_vault.Models;
using key_vault.Initializer.Jwt.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace key_vault.Initializer.Jwt
{
    public class Encryption : IEncryption
    {
        private readonly APIEnvironment Environment;

        public Encryption(APIEnvironment env)
        {
            Environment = env;
		}

        public string GenerateToken(int accountId, Guid tenantId, Guid clientId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Environment.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(nameof(Account.AccountId), accountId.ToString()),
                    new Claim(nameof(Account.TenantId), tenantId.ToString()),
                    new Claim(nameof(Account.ClientId), clientId.ToString())
                }),
                Issuer = Environment.JWTIssuer,
                Audience = Environment.JWTAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool IsTokenValid(string token)
        {
            string tokenSecret = Environment.Secret;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));
            var issuer = Environment.JWTIssuer;

            var validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = issuer,
                ValidAudience = Environment.JWTAudience,
                IssuerSigningKey = key
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                return securityToken != null;
            }
            catch
            {
                return false;
            }
        }

        private byte[] DeriveKeyFromPassword(string password)
        {
            var emptySalt = Array.Empty<byte>();
            var iterations = 1000;
            var desiredKeyLength = 16; // 16 bytes equal 128 bits.
            var hashMethod = HashAlgorithmName.SHA384;
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), emptySalt, iterations, hashMethod, desiredKeyLength);
        }

        public string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(Environment.Secret);
            aes.IV = DeriveKeyFromPassword(Environment.Secret);
            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(Encoding.UTF8.GetBytes(value));
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(output.ToArray());
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(Environment.Secret);
            aes.IV = DeriveKeyFromPassword(Environment.Secret);
            using MemoryStream input = new(Convert.FromBase64String(value));
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            cryptoStream.CopyToAsync(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
