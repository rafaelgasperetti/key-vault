using key_vault.Controllers.Interfaces;
using key_vault.Models;
using key_vault.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    public class SecretController : BaseController
    {
        private readonly ISecretService Service;

        public SecretController(ISecretService service)
        {
            Service = service;
        }

        [HttpGet("secrets/{name}/{version?}")]
        public async Task<SecretResponse> Get(string name, string? version)
        {
            int? accountId = GetAccountId();
            return await Service.Get(accountId, name, version);
        }

        [HttpPut("secrets/{name}")]
        public async Task<SecretResponse> Create(string name, [FromBody] SecretRequest request)
        {
            int? accountId = GetAccountId();
            return await Service.Create(accountId, name, request);
        }

        [HttpDelete("secrets/{name}")]
        public async Task Delete(string name)
        {
            int? accountId = GetAccountId();
            await Service.Delete(accountId.Value, name);
        }
    }
}
