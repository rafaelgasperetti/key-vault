using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/secret")]
    public class SecretController : ControllerBase
    {
        [HttpGet("get/{key}")]
        public string Get(string key)
        {
            return key;
        }

        [HttpPost("create")]
        //[ValidateAntiForgeryToken]
        public void Create(string key, string value)
        {
            
        }

        [HttpDelete("delete/{key}")]
        //[ValidateAntiForgeryToken]
        public void Delete(string key)
        {
            
        }
    }
}
