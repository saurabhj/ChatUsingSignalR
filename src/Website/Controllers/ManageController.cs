using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Website.Models;

namespace Website.Controllers {
    [Authorize]
    public class ManageController : Controller {
        private AuthServiceClient _AuthServiceClient;
        private AuthServiceClient AuthServiceClient {
            get { return _AuthServiceClient ?? (_AuthServiceClient = new AuthServiceClient()); }
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword() {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var result = AuthServiceClient.ChangePassword(User.Identity.Name, model.NewPassword);
            if (result) {
                return RedirectToAction("PasswordChanged", "Account");
            }

            AddErrors(new IdentityResult(new string[] {"The password could not be changed. Please try again."}));
            return View(model);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error);
            }
        }


        #endregion
    }
}