using key_vault.Controllers.Interfaces;
using key_vault.Models;
using key_vault.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService Service;

        public AccountController(IAccountService service)
        {
            Service = service;
        }

        [HttpGet("accounts")]
        public Account Get()
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return null;
            }

            return Service.Get(accountId.Value);
        }

        [AllowAnonymous]
        [HttpPost("accounts")]
        public Account Create([FromBody] Account account)
        {
            return Service.Create(account);
        }

        [HttpDelete("accounts")]
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
