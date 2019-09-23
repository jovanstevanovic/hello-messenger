using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using Microsoft.AspNet.SignalR;

namespace BrzeBoljeJeftinije.Messenger.UI
{
    public class MessengerHub : Hub
    {
        public void Refresh()
        {
            Clients.All.Refresh();
        }

        public override Task OnConnected()
        {
            var sessionId = HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value;
            Groups.Add(Context.ConnectionId, sessionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public static void CallRefresh(string id)
        {
            if (id == null) return;
            GlobalHost.ConnectionManager.GetHubContext<MessengerHub>().Clients.Group(id).Refresh();
        }
    }
}