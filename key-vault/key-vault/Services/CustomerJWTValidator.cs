using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace key_vault.Services
{
    public class CustomJWTValidator : ISecurityTokenValidator
    {
        public bool CanValidateToken => true;

        bool ISecurityTokenValidator.CanValidateToken => true;

        private int _MaximumTokenSizeInBytes;
        int ISecurityTokenValidator.MaximumTokenSizeInBytes { get => _MaximumTokenSizeInBytes; set => _MaximumTokenSizeInBytes = value; }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(securityToken, validationParameters, out SecurityToken _);

            return principal;
        }
    }
}
