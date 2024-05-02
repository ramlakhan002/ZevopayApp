using Microsoft.AspNetCore.Mvc;

namespace Zevopay.Controllers.MVC
{
    public class CallBackController : Controller
    {
        public IActionResult Virtual()
        {
            return View();
        }
    }
}
