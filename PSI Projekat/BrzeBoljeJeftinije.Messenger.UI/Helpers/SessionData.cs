/**
 * SessionData.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrzeBoljeJeftinije.Messenger.UI.Helpers
{
    /**
     * <summary>Helper klasa za pristup podacima iz sesije</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public static class SessionData
    {
        public static string SessionId
        {
            get
            {
                if(HttpContext.Current.Request.Cookies["ASP.NET_SessionId"]!=null)
                {
                    return HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string SignatureAuthToken
        {
            get
            {
                return HttpContext.Current.Session["SignatureAuthToken"] as string;
            }
            set
            {
                HttpContext.Current.Session["SignatureAuthToken"] = value;
            }
        }
        public static User User
        {
            get
            {
                return HttpContext.Current.Session["User"] as User;
            }
            set
            {
                HttpContext.Current.Session["User"] = value;
            }
        }
        public static AdminUser AdminUser
        {
            get
            {
                return HttpContext.Current.Session["AdminUser"] as AdminUser;
            }
            set
            {
                HttpContext.Current.Session["AdminUser"] = value;
            }
        }
    }
}