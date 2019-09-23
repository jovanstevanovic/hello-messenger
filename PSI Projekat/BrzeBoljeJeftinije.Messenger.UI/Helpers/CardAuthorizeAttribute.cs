/**
 * CardAuthorizeAttribute.cs
 * Autor: Nikola Pavlović
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Helpers
{
    /**
     * <summary>Atribut koji označava akcije/kontrolere koje zahtvaju prijavljenog korisnika</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class CardAuthorizeAttribute: AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            return SessionData.User != null || SessionData.AdminUser!=null;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Home");
        }
    }
}