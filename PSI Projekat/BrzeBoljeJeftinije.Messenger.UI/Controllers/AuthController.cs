/**
 *  AuthController.cs
 *  Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB;
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using BrzeBoljeJeftinije.Messenger.UI.Helpers;
using BrzeBoljeJeftinije.Messenger.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     *  <summary>Klasa za prijavu, odjavu i kreiranje naloga korisnika</summary>
     *  
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class AuthController : BaseController
    {
        public AuthController(IDBProvider dbProvider)
            : base(dbProvider)
        {
        }

        /**
         * <summary>izvršava prijavu i eventualno kreiranje korisničkog naloga</summary>
         * <param name="model">Podaci iz HTTP zahteva</param>
         */
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.Signature=="sim")
                {
                    if (!Config.SimLoginEnabled) return HttpNotFound();
                    var name = model.Certificate.Split(':')[1];
                    var id= model.Certificate.Split(':')[0];
                    var secret = model.Certificate.Split(':')[2];
                    var user = GetOrCreateUser(name, id, secret);
                    if (user == null) return HttpNotFound();
                    if(user.BannedUntil!=null && user.BannedUntil>DateTime.Now.Date)
                    {
                        return Content("FAIL:Vaš korisnički nalog je blokiran do " + ((DateTime)user.BannedUntil).ToString("dd.MM.yyyy."));
                    }
                    SessionData.User = user;
                    return Content("OK");
                }
                else
                {
                    var certificate = CryptoHelper.LoadCert(model.Certificate);
                    if (!CryptoHelper.ValidateCert(certificate) || !CryptoHelper.VerifySignature(certificate, SessionData.SignatureAuthToken, model.Signature))
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        SessionData.SignatureAuthToken = null;
                        var user = GetOrCreateUser(certificate);
                        if (user.BannedUntil != null && user.BannedUntil > DateTime.Now.Date)
                        {
                            return Content("FAIL:Vaš korisnički nalog je blokiran do " + ((DateTime)user.BannedUntil).ToString("dd.MM.yyyy."));
                        }
                        SessionData.User = user;
                        return Content("OK");
                    }
                }
            }
            else
            {
                return Content("FAIL:Došlo je do nepoznate greške");
            }
        }

        /**
         * <summary>Izvršava odjavu korisnika</summary>
         */
        [HttpGet]
        [CardAuthorize]
        public ActionResult Logout()
        {
            SessionData.User = null;
            return RedirectToAction("Index", "Home");
        }

        /**
         * <summary>Vraća objekat simulacionog korisnika iz baze, po potrebi ga kreira</summary>
         * <param name="simName">Automatski generisano ime</param>
         * <param name="simId">ID koji se koristi umesto serijskog broja sertifikata</param>
         * <param name="secret">Tajni, automatski generisani podatak, koristi se da bi se
         * koliko toliko sprečila krađa simulacionih naloga</param>
         * <returns>Objekat koji predstavlja korisnika, null ako operacija ne uspe</returns>
         */
        private User GetOrCreateUser(string simName, string simId, string secret)
        {
            if (!simId.StartsWith("SIM")) return null;
            var user = dbProvider.GetUserByCertHash(simId, false);
            if (user != null)
            {
                if (!Encoding.UTF8.GetString(user.Certificate).StartsWith(secret)) return null;
                user.RtID = SessionData.SessionId;
                dbProvider.UpdateUser(user);
                return user;
            }
            user = new User
            {
                CertHash = simId,
                Certificate = Encoding.UTF8.GetBytes(secret),
                Name = simName,
                RtID = SessionData.SessionId
            };
            user.PictureType = "image/png";
            user.Picture = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(@"~/Content/img/user.png"));
            dbProvider.StoreUser(user);
            return user;
        }

        /**
         * <summary>Dohvata (po potrebi kreira) objekat korisnika koji odgovara učitanom sertifikatu sa
         * lične karte</summary>
         * <param name="cert">Sertifikat očitan sa lične karte</param>
         * <returns>Objekat koji predstavlja korisnika, null ako operacija ne uspe</returns>
         */
        private User GetOrCreateUser(X509Certificate2 cert)
        {
            var user = dbProvider.GetUserByCertHash(cert.GetSerialNumberString(), false);
            if (user != null)
            {
                user.RtID= SessionData.SessionId;
                dbProvider.UpdateUser(user);
                return user;
            }
            user = CryptoHelper.GetUserFromCert(cert);
            user.PictureType = "image/png";
            user.Picture = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(@"~/Content/img/user.png"));
            user.RtID = SessionData.SessionId;
            dbProvider.StoreUser(user);
            return user;
        }

        /**
         * <summary>Vraća token koji treba potpisati prilikom prijave ličnom kartom</summary>
         */
        [HttpPost]
        public ActionResult GetSignatureToken()
        {
            if (SessionData.SignatureAuthToken == null)
            {
                SessionData.SignatureAuthToken = CryptoHelper.GetRandomToken();
            }
            return Content(SessionData.SignatureAuthToken);
        }
    }
}