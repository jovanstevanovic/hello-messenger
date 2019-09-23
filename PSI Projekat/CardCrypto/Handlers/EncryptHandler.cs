using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace CardCrypto.Handlers
{
    class EncryptHandler : IRequestHandler
    {
        public string MsgType
        {
            get
            {
                return "encrypt";
            }
        }

        public bool Handle(JObject request, JObject reply)
        {
            string payload = request["payload"].ToObject<String>();
            RSACryptoServiceProvider provider = null;
            if (request.ContainsKey("cert"))
            {
                var cert = request["cert"].ToObject<String>();
                provider = (RSACryptoServiceProvider)CryptoHelper.LoadCert(cert).PublicKey.Key;
            }
            else
            {
                provider = CryptoHelper.GetProvider(false);
            }
            if(provider==null)
            {
                reply["status"] = "NO_CARD";
                return false;
            }
            var data = Convert.FromBase64String(payload);
            var ciphertext = provider.Encrypt(data, false);
            reply["payload"] = Convert.ToBase64String(ciphertext);
            return true;
        }
    }
}
