using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartupAttribute(typeof(Website.Startup))]
namespace Website {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuthentication(app);
            app.MapSignalR();
        }

        public void ConfigureAuthentication(IAppBuilder app) {
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}
