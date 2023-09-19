using key_vault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers.Interfaces
{
    [Authorize]
    [ApiController]
    [ApiVersion(API_VERSION)]
    public class BaseController : ControllerBase
    {
        public const string API_VERSION = "7.4";

        protected int? GetAccountId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.AccountId));

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return int.Parse(claim.Value);
            }

            return null;
        }

        protected Guid? GetTenantId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.TenantId));

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return Guid.Parse(claim.Value);
            }

            return null;
        }

        protected Guid? GetClientId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.ClientId));

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return Guid.Parse(claim.Value);
            }

            return null;
        }

        protected string? GetClientSecret()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.ClientSecret));

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return claim.Value;
            }

            return null;
        }

        public static int GetMajorVersion()
        {
            return Convert.ToInt32(API_VERSION.Contains('.') ? API_VERSION.Split(".")[0] : API_VERSION);
        }

        public static int GetMinorVersion()
        {
            return Convert.ToInt32(API_VERSION.Contains('.') ? API_VERSION.Split(".")[1] : API_VERSION);
        }
    }
}
