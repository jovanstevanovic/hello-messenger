/**
 * UserController.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using BrzeBoljeJeftinije.Messenger.UI.Models;
using BrzeBoljeJeftinije.Messenger.UI.SignalR;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     * <summary>Sadrži akcije za prikaz i promenu informacija o korisničkom nalogu</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    [CardAuthorize]
    public class UserController : BaseController
    {
        public UserController(IDBProvider dbProvider)
            : base(dbProvider)
        {
        }

        /**
         * <summary>Prikazuje sliku ulogovanog korisnika</summary>
         */
        [HttpGet]
        public ActionResult MyPicture()
        {
            var user = dbProvider.GetUserByID(SessionData.User.Id, true);
            return File(user.Picture, user.PictureType);
        }

        /**
         * <summary>Prikazuje sliku korisnika</summary>
         * 
         * <param name="id">Id korisnika</param>
         */
        [HttpGet]
        public ActionResult Picture(int? id)
        {
            if (id == null) return HttpNotFound();
            var user = dbProvider.GetUserByID((int)id, true);
            if (user == null) return HttpNotFound();
            return File(user.Picture, user.PictureType);
        }

        /**
         * <summary>Šalje zahtev za prijateljstvo zadatom korisniku</summary>
         * 
         * <param name="id">Id korisnika</param>
         */
        [HttpPost]
        public ActionResult SendRequest(int? id)
        {
            if (id == null) return HttpNotFound();
            if (id == SessionData.User.Id) return HttpNotFound();
            var other = dbProvider.GetUserByID((int)id, false);
            if (other == null) return HttpNotFound();
            if (dbProvider.GetSentFriendRequests(SessionData.User).Any(x => x.ReceiverId == id)
                || dbProvider.GetUnresolvedFriendRequests(SessionData.User).Any(x => x.SenderId == id))
            {
                return HttpNotFound();
            }
            if (dbProvider.GetFriends(SessionData.User).Any(x => x.Id == id))
            {
                return HttpNotFound();
            }
            dbProvider.CreateFriendRequest(new DB.Entities.FriendRequest
            {
                ReceiverId = (int)id,
                Resolved = false,
                Seen = false,
                SenderId = SessionData.User.Id,
                Timestamp = DateTime.Now
            });
            MessengerHub.CallRefresh(other.RtID);
            return Content("OK");
        }

        /**
         * <summary>Prihvata zahtev za prijateljstvo od zadatog korisnika</summary>
         * 
         * <param name="otherId">Id drugog korisnika</param>
         */
        [HttpPost]
        public ActionResult Accept(int? otherId)
        {
            if (otherId == null) return HttpNotFound();
            var request = dbProvider.GetRequestBetween(new DB.Entities.User { Id = (int)otherId }, SessionData.User);
            if (request == null) return HttpNotFound();
            request.Resolved = true;
            var group = dbProvider.CreateGroup(new DB.Entities.Group
            {
                Binary = true,
                IsAdmin = false,
                Name = null,
                Picture = null,
                PictureType = null,
                Timestamp = DateTime.Now
            });
            dbProvider.AddUsersToGroup(new List<DB.Entities.User>()
            {
                SessionData.User,
                new DB.Entities.User
                {
                    Id=(int)otherId
                }
            }, group, false);
            dbProvider.AddFriendship(SessionData.User, new DB.Entities.User
            {
                Id = (int)otherId
            });
            dbProvider.DeleteFriendRequest(request);
            var other = dbProvider.GetUserByID((int)otherId, false);
            MessengerHub.CallRefresh(other.RtID);
            return Content("OK");
        }

        /**
         * <summary>Odbija zahtev za prijateljstvo od zadatog korisnika</summary>
         * 
         * <param name="otherId">Id drugog korisnika</param>
         */
        [HttpPost]
        public ActionResult Reject(int? otherId)
        {
            if (otherId == null) return HttpNotFound();
            var request = dbProvider.GetRequestBetween(new DB.Entities.User { Id = (int)otherId }, SessionData.User);
            if (request == null) return HttpNotFound();
            request.Resolved = true;
            dbProvider.UpdateFriendRequest(request);
            return Content("OK");
        }

        /**
         * <summary>Postavlja novu sliku za zadatog korisnika</summary>
         * <param name="picture">Uploadovana slika</param>
         */
        [HttpPost]
        public ActionResult MyPicture(HttpPostedFileBase picture)
        {
            try
            {
                Image image = ImageManipulation.LoadImage(picture.InputStream);
                if (image==null)
                {
                    TempData["ErrorMessage"] = "Niste poslali sliku";
                    return RedirectToAction("Index", "Settings");
                }
                var user = SessionData.User;
                image = ImageManipulation.ResizeImage(image, 320, 320);
                user.Picture = ImageManipulation.StoreImage(image);
                user.PictureType = ImageManipulation.OutputImageFormat;
                SessionData.User = user;
                dbProvider.UpdateUser(user);
                return RedirectToAction("Index", "Settings");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Slika koju ste poslali nije ispravna";
                return RedirectToAction("Index", "Settings");
            }
        }

        /**
         * <summary>Vrši pretragu korisnika po imenu</summary>
         * <param name="name">Ime za pretragu</param>
         */
        [HttpPost]
        public ActionResult Search(string name)
        {
            if (name == null) return HttpNotFound();
            var users = dbProvider.SearchUsersByName(name);
            var result = users.Where(x => x.Id != SessionData.User.Id).Select(x => new
            {
                id = x.Id,
                name = x.Name,
                picture = Url.Action("Picture", "User", new { id = x.Id, rand = DateTime.Now.ToString() }),
            }).ToList();
            return Json(result);
        }

        /**
         * <summary>Briše prijateljstvo sa zadatim korisnikom</summary>
         * 
         * <param name="id">Id korisnika</param>
         */
        [HttpPost]
        public ActionResult DeleteFriendship(int id)
        {
            var otherUser = dbProvider.GetUserByID(id, false);
            if (otherUser == null) return HttpNotFound();
            dbProvider.DeleteFriendship(SessionData.User, new DB.Entities.User { Id = id });
            dbProvider.DeleteFriendship(new DB.Entities.User { Id = id }, SessionData.User);
            MessengerHub.CallRefresh(otherUser.RtID);
            return Content("OK");
        }

        /**
         * <summary>Briše nalog trenutno ulogovanog korisnika</summary>
         */
        [HttpPost]
        public ActionResult DeleteMe()
        {
            dbProvider.DeleteUser(SessionData.User);
            SessionData.User = null;
            return Content("OK");
        }
    }
}