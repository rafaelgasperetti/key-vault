using Microsoft.AspNetCore.Mvc;

namespace key_vault.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/secret")]
    public class SecretController : ControllerBase
    {
        [HttpGet]
        [ValidateAntiForgeryToken]
        public string Get(string key)
        {
            return key;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Create(string key, string value)
        {
            
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public void Delete(string key)
        {
            
        }
    }
}
