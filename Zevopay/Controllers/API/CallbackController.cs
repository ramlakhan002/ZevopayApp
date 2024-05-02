using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Zevopay.Controllers.API
{
  [Route("api/Callback")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        [HttpGet("GetVirtual")]
        public IActionResult Virtual(int id)
        {
            return Ok(new string[] {"sdgd","asger","fgh"});
        }
    }
}
