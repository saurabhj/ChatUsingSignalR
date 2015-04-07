using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Website.Models;

namespace Website.Controllers {
    [Authorize]
    public class AccountController : Controller {

        IAuthenticationManager Authentication {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private AuthServiceClient _AuthServiceClient;
        private AuthServiceClient AuthServiceClient {
            get { return _AuthServiceClient ?? (_AuthServiceClient = new AuthServiceClient()); }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var result = AuthServiceClient.LoginUser(model.Email, model.Password);
            if (result) {
                var username = AuthServiceClient.GetUsernameFromEmail(model.Email);

                var identity = new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Name, username), },
                        DefaultAuthenticationTypes.ApplicationCookie,
                        ClaimTypes.Name, ClaimTypes.Role);

                Authentication.SignIn(new AuthenticationProperties {
                    IsPersistent = false
                }, identity);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        public ActionResult PasswordChanged() {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model) {
            var errorMsg = string.Empty;

            if (ModelState.IsValid) {
                

                var isRegistered = AuthServiceClient.RegisterUser(
                    username: model.Username,
                    email: model.Email,
                    password: model.Password,
                    errorMessage: out errorMsg);

                if (isRegistered) {
                    // Set the login cookie as well
                    var identity = new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.Name, model.Username), },
                        DefaultAuthenticationTypes.ApplicationCookie,
                        ClaimTypes.Name, ClaimTypes.Role);

                    Authentication.SignIn(new AuthenticationProperties {
                        IsPersistent = false
                    }, identity);

                    return RedirectToAction("Index", "Home");
                }
            }

            AddErrors(new IdentityResult(new string[] { errorMsg }));
            return View(model);
        }



        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
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

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) {
            }

            public ChallengeResult(string provider, string redirectUri, string userId) {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
            
        }
        #endregion
    }
}