using key_vault.Controllers.Interfaces;
using key_vault.Models;
using key_vault.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    [Route("api/v{version:apiVersion}/secret")]
    public class SecretController : BaseController
    {
        private readonly ISecretService Service;

        public SecretController(ISecretService service)
        {
            Service = service;
        }

        [HttpGet("get/{name}")]
        public SecretKey Get(string name)
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return null;
            }

            return Service.Get(accountId.Value, name);
        }

        [HttpPost("create")]
        public SecretKey Create([FromBody] SecretKey secretKey)
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return null;
            }

            secretKey.AccountId = accountId.Value;
            return Service.Create(secretKey);
        }

        [HttpDelete("delete/{name}")]
        public void Delete(string name)
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return;
            }

            Service.Delete(accountId.Value, name);
        }
    }
}
