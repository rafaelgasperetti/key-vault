﻿using key_vault.Controllers.Interfaces;
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
        public SecretResponse Get(string name, string? version)
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return null;
            }

            return Service.Get(accountId.Value, name, version);
        }

        [HttpPut("secrets/{name}")]
        public SecretResponse Create(string name, [FromBody] SecretKey secretKey)
        {
            int? accountId = GetAccountId();

            if (accountId == null)
            {
                return null;
            }

            secretKey.AccountId = accountId.Value;
            return Service.Create(name, secretKey);
        }

        [HttpDelete("secrets/{name}")]
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