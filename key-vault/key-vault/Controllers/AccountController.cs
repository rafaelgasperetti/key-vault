using key_vault.Controllers.Interfaces;
using key_vault.Models;
using key_vault.Properties;
using key_vault.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService Service;

        public AccountController(APIEnvironment environment, IAccountService service) : base(environment)
        {
            Service = service;
        }

        [AllowAnonymous]
        [HttpGet("/")]
        public string Index()
        {
            return string.Format(Strings.ApplicationRunningMessage, APIEnvironment.Version.ToString());
        }

        [HttpGet("accounts")]
        public async Task<Account> Get()
        {
            int? accountId = GetAccountId();
            return await Service.Get(accountId);
        }

        [AllowAnonymous]
        [HttpPost("accounts")]
        public async Task<Account> Create([FromBody] Account account)
        {
            return await Service.Create(account);
        }

        [HttpDelete("accounts")]
        public async Task Delete()
        {
            int? accountId = GetAccountId();
            await Service.Delete(accountId);
        }
    }
}
