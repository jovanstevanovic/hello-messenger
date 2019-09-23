/**
 * SettingsController.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     * <summary>Prikazuje stranicu za podešavanja</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    [CardAuthorize]
    public class SettingsController : Controller
    {
        /**
         * <summary>Prikazuje stranicu za podešavanja</summary>
         */
        public ActionResult Index()
        {
            return View();
        }
    }
}