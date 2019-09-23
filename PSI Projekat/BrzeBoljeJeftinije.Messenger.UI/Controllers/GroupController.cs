/**
 * GroupController.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB;
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using BrzeBoljeJeftinije.Messenger.UI.Models;
using BrzeBoljeJeftinije.Messenger.UI.SignalR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     * <summary>Sadrži akcije za upravljanje i pregled korisničkih grupa</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    [CardAuthorize]
    public class GroupController : BaseController
    {
        public GroupController(IDBProvider dbProvider) : base(dbProvider)
        {
        }

        /**
         * <summary>Vraća listu grupa kojima pripada trenutni korisnik</summary>
         */
        [HttpPost]
        public ActionResult My()
        {
            var groups = dbProvider.GetGroupsForUser(SessionData.User);
            var requests = dbProvider.GetUnresolvedFriendRequests(SessionData.User).Union(dbProvider.GetSentFriendRequests(SessionData.User)).ToList();
            var friends = dbProvider.GetFriends(SessionData.User);
            groups.Sort((g1, g2) =>
            {
                var t1 = g1.LastMessage != null ? (DateTime)g1.LastMessage : g1.Timestamp;
                var t2 = g2.LastMessage != null ? (DateTime)g2.LastMessage : g2.Timestamp;
                return (int)((t1 - t2).TotalSeconds);
            });
            requests.Sort((r1, r2) =>
            {
                return (int)((r1.Timestamp - r2.Timestamp).TotalSeconds);
            });
            List<object> result = new List<object>();
            foreach (var group in groups)
            {
                var members = dbProvider.GetUsersInGroup(group);
                string pictureUrl = "";
                string groupName = group.Name;
                int? otherId = null;
                if (group.Binary)
                {
                    otherId = members.First(x => x.Id != SessionData.User.Id).Id;
                    pictureUrl = Url.Action("Picture", "User", new { id = otherId, rand = DateTime.Now.ToString() });
                    groupName = members.First(x => x.Id != SessionData.User.Id).Name;
                }
                else
                {
                    pictureUrl = "/Group/Picture/"+group.Id;
                }
                result.Add(new
                {
                    id = group.Id,
                    timestamp = group.LastMessage != null ? group.LastMessage : group.Timestamp,
                    type = "group",
                    unread = group.ContainsUnread,
                    admin = group.IsAdmin,
                    binary = group.Binary,
                    members,
                    picture = pictureUrl,
                    otherId,
                    name = groupName
                });
            }
            foreach (var request in requests)
            {
                result.Add(new
                {
                    id = "r" + request.SenderId,
                    timestamp = request.Timestamp,
                    type = "request",
                    sent = request.SenderId == SessionData.User.Id,
                    admin = false,
                    picture = Url.Action("Picture", "User", new { id = (request.SenderId == SessionData.User.Id ? request.ReceiverId : request.SenderId), rand = DateTime.Now.ToString() }),
                    otherId = request.SenderId == SessionData.User.Id ? request.ReceiverId : request.SenderId,
                    name = dbProvider.GetUserByID(request.SenderId == SessionData.User.Id ? request.ReceiverId : request.SenderId, false).Name,
                });
            }
            return Json(result);
        }

        /**
         * <summary>Prikazuje formu za editovanje postojeće grupe ili prikazuje praznu formu za kreiranje nove grupe
         * </summary>
         * <param name="id">Id grupe koja se edituje, ako je <c>null</c> prikazuje se prazna forma za kreiranje
         * nove grupe</param>
         * 
         */
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id!=null)
            {
                var group = dbProvider.GetGroupsForUser(SessionData.User).FirstOrDefault(x => x.Id == (int)id && (bool)x.IsAdmin);
                if (group == null) return HttpNotFound();
                var model = new GroupModel
                {
                    Id = group.Id,
                    Name = group.Name
                };
                model.SetMembers(dbProvider.GetUsersInGroup(group));
                return PartialView("~/Views/Group/Edit.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Group/Edit.cshtml", new GroupModel
                {
                    Members = "[]",
                    Name = "",
                });
            }
        }

        /**
         * <summary>Edituje postojeću ili kreira novu grupu</summary>
         * <param name="model">Podaci iz HTTP zahteva</param>
         */
        [HttpPost]
        public ActionResult Edit(GroupModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == null)
                {
                    if (model.Picture == null)
                    {
                        return Content("FAIL:Niste poslali sliku");
                    }
                    var image = ImageManipulation.LoadImage(model.Picture.InputStream);
                    if (image == null)
                    {
                        return Content("FAIL:Niste poslali sliku");
                    }
                    image = ImageManipulation.ResizeImage(image, 160, 160);
                    var group = dbProvider.CreateGroup(new DB.Entities.Group
                    {
                        Binary = false,
                        Name = model.Name,
                        Timestamp = DateTime.Now,
                        Picture = ImageManipulation.StoreImage(image),
                        PictureType = ImageManipulation.OutputImageFormat,
                    });
                    var users = model.GetMembers();
                    var friends = dbProvider.GetFriends(SessionData.User).Union(dbProvider.GetUsersInGroup(group));
                    foreach (var user in users)
                    {
                        if (user.Id == SessionData.User.Id || friends.All(x => x.Id != user.Id))
                        {
                            return HttpNotFound();
                        }
                    }
                    dbProvider.AddUsersToGroup(users, group, false);
                    dbProvider.AddUsersToGroup(new List<DB.Entities.User> { SessionData.User }, group, true);
                    dbProvider.CommitIfNecessary();
                    foreach (var user in users)
                    {
                        MessengerHub.CallRefresh(friends.First(x => x.Id == user.Id).RtID);
                    }                    
                    return Content("OK");
                }
                else
                {
                    Image picture = null;
                    if(model.Picture!=null)
                    {
                        picture = ImageManipulation.LoadImage(model.Picture.InputStream);
                        if(picture==null)
                        {
                            return Content("FAIL:Niste poslali sliku");
                        }
                        picture = ImageManipulation.ResizeImage(picture, 160, 160);
                    }
                    var group = dbProvider.GetGroupsForUser(SessionData.User).FirstOrDefault(x=>x.Id==model.Id);
                    if (group == null || !(bool)group.IsAdmin) return HttpNotFound();
                    var oldMembers = dbProvider.GetUsersInGroup(group);
                    var newMembers = model.GetMembers();
                    var friends = dbProvider.GetFriends(SessionData.User).Union(dbProvider.GetUsersInGroup(group));
                    foreach (var user in newMembers)
                    {
                        if (user.Id == SessionData.User.Id || friends.All(x => x.Id != user.Id))
                        {
                            return HttpNotFound();
                        }
                    }
                    newMembers.Add(SessionData.User);
                    List<User> deletedMembers = new List<User>();
                    List<User> addedMembers = new List<User>();
                    foreach(var member in oldMembers)
                    {
                        if (!newMembers.Any(x => x.Id == member.Id)) deletedMembers.Add(member);
                    }
                    foreach (var member in newMembers)
                    {
                        if (!oldMembers.Any(x => x.Id == member.Id)) addedMembers.Add(member);
                    }
                    dbProvider.RemoveUsersFromGroup(deletedMembers, group);
                    dbProvider.AddUsersToGroup(addedMembers, group, false);
                    if(model.Name!=group.Name || model.Picture!=null)
                    {
                        if (model.Name != group.Name) group.Name = model.Name;
                        if (picture != null) group.Picture = ImageManipulation.StoreImage(picture);
                        dbProvider.UpdateGroup(group, picture != null);
                    }
                    dbProvider.CommitIfNecessary();
                    foreach(var member in deletedMembers)
                    {
                        MessengerHub.CallRefresh(member.RtID);
                    }
                    foreach(var member in addedMembers)
                    {
                        MessengerHub.CallRefresh(friends.First(x => x.Id == member.Id).RtID);
                    }
                    return Content("OK");
                }
            }
            else
            {
                var x = ModelValidationErrors;
                return Content("FAIL:"+ string.Join(", ", ModelValidationErrors));
            }
        }

        /**
         * <summary>Prikazuje sliku grupe</summary>
         * <param name="id">Id grupe čija se slika traži</param>
         */
        [HttpGet]
        public ActionResult Picture(int id)
        {
            var groups = dbProvider.GetGroupsForUser(SessionData.User);
            var group = groups.FirstOrDefault(x => x.Id == id);
            if (group == null) return HttpNotFound();
            return new FileContentResult(group.Picture, ImageManipulation.OutputImageFormat);
        }

        /**
         * <summary>Izbacuje trenutnog korisnika iz grupe</summary>
         * <param name="id">Id grupe</param>
         */
        [HttpPost]
        public ActionResult Leave(int id)
        {
            var groups = dbProvider.GetGroupsForUser(SessionData.User);
            var group = groups.FirstOrDefault(x => x.Id == id);
            if (group == null) return HttpNotFound();
            if ((bool)group.IsAdmin) return HttpNotFound();
            dbProvider.RemoveUsersFromGroup(new List<User> { SessionData.User }, group);
            return Content("OK");
        }
    }
}