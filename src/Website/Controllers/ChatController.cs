using System.Web.Mvc;

namespace Website.Controllers {
    [Authorize]
    public class ChatController : Controller {
        // GET: Chat
        public ActionResult Index() {
            return View();
        }
    }
}