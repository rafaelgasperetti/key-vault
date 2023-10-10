using key_vault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers.Interfaces
{
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly APIEnvironment APIEnvironment;

        public BaseController(APIEnvironment environment)
        {
            APIEnvironment = environment;
        }

        protected int? GetAccountId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.AccountId));

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return int.Parse(claim.Value);
            }

            return null;
        }

        protected string? GetAccountName()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == nameof(Account.Name));

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return claim.Value;
            }

            return null;
        }
    }
}
