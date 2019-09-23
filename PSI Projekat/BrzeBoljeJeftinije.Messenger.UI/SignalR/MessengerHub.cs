/**
 * MessengerHub.cs
 * Autor: Nikola Pavlović
 */
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;

namespace BrzeBoljeJeftinije.Messenger.UI.SignalR
{
    /**
     * <summary>Helper klasa za slanje notifikacija preko SignalR mehanizma</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class MessengerHub : Hub
    {
        public override Task OnConnected()
        {
            if(SessionData.SessionId!=null) Groups.Add(Context.ConnectionId, SessionData.SessionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public static void CallRefresh(string id)
        {
            try
            {
                if (id == null) return;
                GlobalHost.ConnectionManager.GetHubContext<MessengerHub>().Clients.Group(id).Refresh();
            }
            catch
            {

            }
        }

        public static void CallNewMessagesForGroup(string user, int group)
        {
            try
            {
                if (user == null) return;
                GlobalHost.ConnectionManager.GetHubContext<MessengerHub>().Clients.Group(user).NewMessagesForGroup(group);
            }
            catch
            {

            }
        }
    }
}