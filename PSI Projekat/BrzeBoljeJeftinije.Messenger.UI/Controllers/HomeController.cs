/**
 * HomeController.cs
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
     *  <summary>Prikazuje početnu stranicu i politiku privatnosti</summary>
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class HomeController : Controller
    {
        /**
         * <summary>Prikazuje početnu stranicu</summary>
         */
        public ActionResult Index()
        {
            return View();
        }

        /**
         * <summary>Prikazuje politiku privatnosti</summary>
         */
        public ActionResult Privacy()
        {
            return View();
        }
    }
}