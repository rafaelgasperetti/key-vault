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
            var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.AccountId));

            if (accountIdClaim != null && !string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return int.Parse(accountIdClaim.Value);
            }

            return null;
        }

        protected Guid? GetAPIKey()
        {
            var apiKeyClaim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.APIKey));

            if (apiKeyClaim != null && !string.IsNullOrEmpty(apiKeyClaim.Value))
            {
                return Guid.Parse(apiKeyClaim.Value);
            }

            return null;
        }
    }
}
