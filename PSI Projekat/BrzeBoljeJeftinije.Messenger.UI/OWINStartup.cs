using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BrzeBoljeJeftinije.Messenger.UI.OWINStartup))]

namespace BrzeBoljeJeftinije.Messenger.UI
{
    public class OWINStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
