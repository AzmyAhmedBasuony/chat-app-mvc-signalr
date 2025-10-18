using Microsoft.AspNetCore.Mvc;

namespace SignalrMvc.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GroupChat()
        {
            // Old group chat without authentication
            return View();
        }
    }
}

