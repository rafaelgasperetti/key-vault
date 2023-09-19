using key_vault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers.Interfaces
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    public class BaseController : ControllerBase
    {
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
    }
}
