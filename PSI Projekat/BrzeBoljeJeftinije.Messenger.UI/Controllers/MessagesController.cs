/**
 * MessagesController.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB;
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using BrzeBoljeJeftinije.Messenger.UI.Models;
using BrzeBoljeJeftinije.Messenger.UI.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     *  Sadrži akcije za slanje i prijem poruka
     *  
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    [CardAuthorize]
    public class MessagesController : BaseController
    {
        public MessagesController(IDBProvider dbProvider)
            : base(dbProvider)
        {
        }

        /**
         * <summary>Prikazuje stranicu sa porukama</summary>
         */
        public ActionResult Index()
        {
            var user = SessionData.User;
            user.RtID = SessionData.SessionId;
            dbProvider.UpdateUser(user);
            SessionData.User = user;
            return View();
        }

        /**
         * <summary>Vraća javne ključeve svih korisnika koji pripadaju grupi</summary>
         * <param name="id">Id grupe</param>
         */
        [HttpPost]
        public ActionResult GetPksForGroup(int id)
        {
            var members = dbProvider.GetUsersInGroup(new DB.Entities.Group { Id = id });
            if (!members.Any(x => x.Id == SessionData.User.Id)) return HttpNotFound();
            List<object> pks = new List<object>();
            foreach(var user in members)
            {
                if(user.CertHash.StartsWith("SIM-"))
                {
                    pks.Add(new { id = user.Id, key = "sim" });
                }
                else
                {
                    pks.Add(new { id = user.Id, key = CryptoHelper.LoadCert(user.Certificate).PublicKey.Key.ToXmlString(false) });
                }
            }
            return Json(pks);
        }

        /**
         * <summary>Šalje poruku</summary>
         * <param name="model">Parametri iz HTTP zahteva</param>
         */
        [HttpPost]
        public ActionResult Send(SentMessageModel model)
        {
            if(ModelState.IsValid)
            {
                var groupMembers = dbProvider.GetUsersInGroup(new Group { Id = (int)model.GroupId });
                if (groupMembers.All(x => x.Id != SessionData.User.Id)) return HttpNotFound();
                var materials = JsonConvert.DeserializeObject<Dictionary<int, string>>(model.Materials);
                var message = new Message
                {
                    GroupId = (int)model.GroupId,
                    SenderId = SessionData.User.Id,
                    TimeStamp = DateTime.Now,
                    Text = model.Ciphertext
                };
                dbProvider.StoreMessage(message);
                foreach(var material in materials)
                {
                    if(groupMembers.Any(x=>x.Id==material.Key))
                    {
                        dbProvider.StoreCryptographicMaterial(new MessageCryptoMaterial
                        {
                            Material = material.Value,
                            MessageId = message.Id,
                            UserId = material.Key
                        });
                    }
                }
                if(model.Attachment!=null && model.AttachmentName!=null && model.AttachmentType!=null)
                {
                    dbProvider.StoreAttachment(new Attachment
                    {
                        Content = Encoding.UTF8.GetBytes(model.Attachment),
                        FileExtension = model.AttachmentType,
                        FileName = model.AttachmentName,
                        MessageId = message.Id
                    });
                }
                foreach(var member in groupMembers)
                {
                    MessengerHub.CallNewMessagesForGroup(member.RtID, (int)model.GroupId);
                }
                return Content("OK");
            }
            else
            {
                return HttpNotFound();
            }
        }

        /**
         * <summary>Dohvata poruke iz zadate grupe</summary>
         * 
         * <param name="groupId">Id grupe</param>
         * <param name="page">Redni broj stranice - za paginaciju, indeksirano od nule</param>
         */
        [HttpPost]
        public ActionResult GetMessages(int groupId, int page)
        {
            if (!dbProvider.GetUsersInGroup(new Group { Id = groupId }).Any(x => x.Id == SessionData.User.Id)) return HttpNotFound();
            var messages = dbProvider.GetMessagesForGroup(new Group { Id = groupId }, Math.Max(page, 0), 10, SessionData.User);
            var materials = new List<MessageCryptoMaterial>();
            messages.ForEach(x => { var mat = dbProvider.GetCryptographicMaterial(SessionData.User, x); if (mat != null) materials.Add(mat); });
            messages = messages.Where(x => materials.Any(y => y.MessageId == x.Id)).ToList();
            List<object> result = new List<object>();
            foreach(var message in messages)
            {
                result.Add(new
                {
                    Id = message.Id,
                    Content = message.Text,
                    Sender = message.SenderId,
                    Time = message.TimeStamp,
                    CryptoMaterial = materials.First(x=>x.MessageId==message.Id).Material,
                    Attachments = dbProvider.GetAttachmentsForMessage(message).Select(x => new
                    {
                        Name = x.FileName,
                        Type = x.FileExtension,
                        Id = x.Id
                    }).ToList()
                });
            }
            return Json(result);
        }

        /**
         * <summary>Dohvata sadržaj attachment-a</summary>
         * <param name="id">Id attachment-a</param>
         * <param name="messageId">Id poruke kojoj pripada attachment</param>
         */
        [HttpPost]
        public ActionResult GetAttachment(int messageId, int id)
        {

            var attachment = dbProvider.GetAttachmentById(id);
            if (attachment == null) return HttpNotFound();
            var message = dbProvider.GetMessageById(messageId);
            if (!dbProvider.GetGroupsForUser(SessionData.User).Any(x => x.Id == message.GroupId)) return HttpNotFound();
            return Content(Encoding.UTF8.GetString(attachment.Content));
        }

        /**
         * <summary>Briše poruku</summary>
         * <param name="id">Id poruke</param>
         */
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var message = dbProvider.GetMessageById(id);
            if (message == null || message.SenderId != SessionData.User.Id) return HttpNotFound();
            dbProvider.DeleteMessage(id);
            return Content("OK");
        }
    }
}
 