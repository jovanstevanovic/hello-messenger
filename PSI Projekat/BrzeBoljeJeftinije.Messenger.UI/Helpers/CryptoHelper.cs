/**
 * CryptoHelper.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace BrzeBoljeJeftinije.Messenger.UI.Helpers
{
    /**
     * <summary>Helper klasa za kriptografske operacije</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public static class CryptoHelper
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static X509Chain _mupChain = null;
        private static X509Certificate2[] _authority = null;
        private static X509Certificate2[] authority
        {
            get
            {
                if (_authority != null) return _authority;
                _authority = GetMUPRSCert();
                return _authority;
            }
        }
        private static X509Chain mupChain
        {
            get
            {
                if (_mupChain != null) return _mupChain;
                X509Chain chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = DateTime.Now;

                foreach (var crt in authority)
                {
                    chain.ChainPolicy.ExtraStore.Add(crt);
                }
                _mupChain = chain;
                return _mupChain;
            }
        }
        public static string GetRandomToken(int length=16)
        {
            byte[] buffer = new byte[length];
            rngCsp.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }
        public static X509Certificate2 LoadCert(byte[] cert)
        {
            return new X509Certificate2(cert);
        }
        public static X509Certificate2 LoadCert(string cert)
        {
            return LoadCert(Convert.FromBase64String(cert));
        }
        private static X509Certificate2[] GetMUPRSCert()
        {
            var basePath = HostingEnvironment.MapPath(@"~/Certs/");
            var certs = new string[] { "MUPCAGradjani.crt", "MUPCAGradjani2.crt", "MUPCAGradjani3.crt", "MUPCARoot.crt", "MUPCARoot3.crt" };
            List<X509Certificate2> returned = new List<X509Certificate2>();
            foreach (var cert in certs)
            {
                var path = Path.Combine(basePath, cert);
                returned.Add(new X509Certificate2(File.ReadAllBytes(path)));
            }
            return returned.ToArray();
        }
        public static bool ValidateCert(X509Certificate2 certificateToValidate)
        {
            if (certificateToValidate == null) return false;
            if (!(certificateToValidate.PublicKey.Key is RSACryptoServiceProvider)) return false;
            
            bool isChainValid = mupChain.Build(certificateToValidate);

            if (!isChainValid)
            {
                return false;
            }

            var valid = mupChain.ChainElements
                .Cast<X509ChainElement>()
                .Any(x => authority.Any(y => x.Certificate.Thumbprint == y.Thumbprint));

            return valid;
        }
        public static bool VerifySignature(X509Certificate2 certificate, string payload, string signature64)
        {
            byte[] signature = Convert.FromBase64String(signature64);
            RSACryptoServiceProvider provider = certificate.PublicKey.Key as RSACryptoServiceProvider;
            if (provider != null)
            {
                var data = Convert.FromBase64String(payload);
                bool result = provider.VerifyData(data, CryptoConfig.MapNameToOID("SHA256"), signature);
                return result;
            }
            else
            {
                return false;
            }
        }
        public static User GetUserFromCert(X509Certificate2 cert)
        {
            var nameRegex = new Regex(@"[A-Za-zАБВГДЂЕЖЗИЈКЛЉМНЊОПРСТЋУФХЦЧЏШабвгдђежзијклљмнњопрстћуфхцчџшČĆŽŠĐčćžšđ]+\s[A-Za-zАБВГДЂЕЖЗИЈКЛЉМНЊОПРСТЋУФХЦЧЏШабвгдђежзијклљмнњопрстћуфхцчџшČĆŽŠĐčćžšđ]+");
            var name = nameRegex.Match(cert.Subject);
            return new User
            {
                CertHash = cert.GetSerialNumberString(),
                Certificate = cert.Export(X509ContentType.Cert),
                Name = name.Value
            };
        }
    }
}