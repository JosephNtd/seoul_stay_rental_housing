using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(GUI_Web_Payment.Startup))]

namespace GUI_Web_Payment
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            app.MapSignalR();
        }
    }
}