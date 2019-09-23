/**
 * AuthController.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using BrzeBoljeJeftinije.Messenger.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     * <summary>Klasa za prijavu/odjavu administratora i akcije koje obavlja administrator</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class AdminController : BaseController
    {

        public AdminController(IDBProvider dbProvider)
            : base(dbProvider)
        {
        }

        /**
         * <summary>Prikazuje stranicu sa koje administratori vrše upravljanje korisnicima</summary>
         */
        [AdminAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        /**
         * <summary>Prikazuje stranicu za prijavu administratora</summary>
         */
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /**
         * <summary>Prijavljuje administratora na sistem</summary>
         * <param name="model">Podaci iz HTTP zahteva</param>
         */
        [HttpPost]
        public ActionResult Login(AdminUser model)
        {
            var user = dbProvider.GetAdminUser(model.Username);
            if (user == null || !user.ValidatePassword(model.Password))
            {
                ViewData["AdminError"] = "Neispravno ime ili lozinka";
                return View();
            }
            SessionData.AdminUser = user;
            return Redirect(Url.Action("Index"));
        }

        /**
         * <summary>Odjavljuje administratora sa sistema</summary>
         */
        [HttpGet]
        [AdminAuthorize]
        public ActionResult Logout()
        {
            SessionData.AdminUser = null;
            return Redirect(Url.Action("Login"));
        }

        /**
         * <summary>Pretražuje korisnike sa zadatim imenom</summary>
         */
        [HttpPost]
        [AdminAuthorize]
        public ActionResult Search(string name)
        {
            if (name == null) return HttpNotFound();
            var users = dbProvider.SearchUsersByName(name);
            var result = users.Select(x => new
            {
                id = x.Id,
                name = x.Name,
                picture = Url.Action("Picture", "User", new {id=x.Id, rand=DateTime.Now.ToString()}),
                banDate= x.BannedUntil==null?DateTime.Now:x.BannedUntil,
                status= (x.BannedUntil==null || x.BannedUntil<=DateTime.Now.Date)?"ok":"ban",
            }).ToList();
            return Json(result);
        }

        /**
         * <summary>Menja status (banuje, odbanuje, menja datum bana) zadatom korisniku</summary>
         * <param name="model">Podaci iz HTTP zahteva</param>
         */
        [HttpPost]
        [AdminAuthorize]
        public ActionResult UpdateStatus(UpdateStatusModel model)
        {
            if (!ModelState.IsValid) return HttpNotFound();
            var user = dbProvider.GetUserByID(model.Id, false);
            if (user == null) return HttpNotFound();
            if(model.Status=="ok")
            {
                user.BannedUntil = null;
                dbProvider.UpdateUser(user);
                return Content("OK");
            }
            else
            {
                if(model.ExpiryDate<=DateTime.Now)
                {
                    return Content("FAIL:Datum bana je neispravan");
                }
                user.BannedUntil = model.ExpiryDate.Date;
                dbProvider.UpdateUser(user);
                return Content("OK");
            }
        }

        /**
         * <summary>Setuje novu lozinku ulogovanom korisniku</summary>
         * <param name="model">Podaci iz HTTP zahteva</param>
         */
        [HttpPost]
        [AdminAuthorize]
        public ActionResult MyPassword(string password)
        {
            if (password == null) return HttpNotFound();
            if (password.Length == 0) return HttpNotFound();
            var user = SessionData.AdminUser;
            user.SetPassword(password);
            SessionData.AdminUser = user;
            dbProvider.UpdateAdminUser(user);
            return Content("OK");
        }

        /**
         * <summary>Registruje novog administratora</summary>
         * <param name="model">Podaci iz HTTP zahteva</param>
         */
        [HttpPost]
        [AdminAuthorize]
        public ActionResult Register(AdminUser user)
        {
            if(string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return HttpNotFound();
            }
            var oldUser = dbProvider.GetAdminUser(user.Username);
            if(oldUser!=null)
            {
                return Content("FAIL:Već postoji administrator pod tim imenom");
            }
            var newAdmin = new DB.Entities.AdminUser
            {
                Username=user.Username
            };
            newAdmin.SetPassword(user.Password);
            dbProvider.StoreAdminUser(newAdmin);
            return Content("OK");
            
        }
    }
}
 