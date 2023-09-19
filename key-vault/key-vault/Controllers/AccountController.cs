using key_vault.Controllers.Interfaces;
using key_vault.Models;
using key_vault.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    [Route("api/v{version:apiVersion}/account")]
    public class AccountController : BaseController
    {
        private readonly IAccountService Service;

        public AccountController(IAccountService service)
        {
            Service = service;
        }

        [AllowAnonymous]
        [HttpGet("get/{accountId}/{apiKey}")]
        public Account Get(int accountId, string apiKey)
        {
            return Service.Get(accountId, apiKey);
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public Account Create([FromBody] Account account)
        {
            return Service.Create(account);
        }

        [HttpDelete("delete")]
        public void Delete()
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return;
            }

            Service.Delete(accountId.Value);
        }
    }
}
