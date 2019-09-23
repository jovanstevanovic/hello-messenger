/**
 * Program.cs
 * Autor: Nikola Pavlović
 */
using Chrome4Net.NativeMessaging;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CardCrypto
{
    /**
     * <summary>Program koji prima zahteve od ekstenzije i obrađuje ih</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    class NativeHost
    {
        private static ILog log = null;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(NativeHost));
            runNativeHost();
        }
        private static void runNativeHost()
        {
            Port port = new Port();
            log.Info("Running native");
            while (true)
            {
                try
                {
                    string message = port.Read();
                    if(message.Length>200)
                    {
                        log.Info("Got message: "+message.Substring(0,200)+"...");
                    }
                    else
                    {
                        log.Info("Got message: " + message);
                    }
                    JObject request = JObject.Parse(message);
                    JObject reply = new JObject();
                    try
                    {
                        if (request.ContainsKey("type"))
                        {
                            string type = request["type"].ToObject<String>();
                            switch(type)
                            {
                                case "getAesKey":
                                    getAesKey(request, reply);
                                    break;
                                case "aesEncrypt":
                                    aesEncrypt(request, reply);
                                    break;
                                case "aesDecrypt":
                                    aesDecrypt(request, reply);
                                    break;
                                case "rsaEncrypt":
                                    rsaEncrypt(request, reply);
                                    break;
                                case "rsaDecrypt":
                                    rsaDecrypt(request, reply);
                                    break;
                                case "rsaDecryptMulti":
                                    rsaDecryptMulti(request, reply);
                                    break;
                                case "getCertificate":
                                    getCertificate(request, reply);
                                    break;
                                case "getPublic":
                                    getPublic(request, reply);
                                    break;
                                case "sign":
                                    sign(request, reply);
                                    break;
                                case "verify":
                                    verify(request, reply);
                                    break;
                                case "check":
                                    check(request, reply);
                                    break;
                            }
                            if(!reply.ContainsKey("status")) reply["status"] = "OK";
                        }
                    }
                    catch (Exception ex)
                    {
                        reply["status"] = "EXC";
                        reply["message"] = ex.Message;
                        reply["strace"] = ex.StackTrace;
                    }
                    reply["source"] = request["destination"];
                    reply["destination"] = request["source"];
                    reply["extension"] = "brzeboljejeftinije.messenger.cardreader";
                    request.Remove("payload");
                    reply["request"] = request;
                    message = reply.ToString(Formatting.None);
                    port.Write(message);
                }
                catch(EndOfInputStreamException)
                {
                    return;
                }
            }
        }

        private static void rsaDecryptMulti(JObject request, JObject reply)
        {
            requireFields(request, "payload");
            if (!CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }
            var payloads = request["payload"].ToObject<String[]>();
            var plaintexts = new List<string>();
            foreach(var payload in payloads)
            {
                plaintexts.Add(CryptoHelper.RSADecrypt(payload));
            }
            reply["payload"] = new JArray(CryptoHelper.RSADecryptMulti(payloads));
        }

        private static void check(JObject request, JObject reply)
        {
            if(CryptoHelper.CardPresentAndCertValid())
            {
                reply["payload"] = "true";
            }
            else
            {
                reply["payload"] = "false";
            }
        }

        private static void sign(JObject request, JObject reply)
        {
            requireFields(request, "payload");
            if (!CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }
            string plaintext = CryptoHelper.Sign(request["payload"].ToObject<String>());
            reply["payload"] = plaintext;
        }

        private static void verify(JObject request, JObject reply)
        {
            requireFields(request, "payload", "signature");
            string payload = request["payload"].ToObject<String>();
            string signature = request["signature"].ToObject<String>();
            string cert = request.ContainsKey("cert") ? request["cert"].ToObject<String>() : null;
            string key = request.ContainsKey("key") ? request["key"].ToObject<String>() : null;
            if (cert == null && key == null && !CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }

            reply["payload"] = CryptoHelper.Verify(payload, signature, cert, key);
        }

        private static void rsaDecrypt(JObject request, JObject reply)
        {
            requireFields(request, "payload");
            if(!CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }
            string plaintext = CryptoHelper.RSADecrypt(request["payload"].ToObject<String>());
            reply["payload"] = plaintext;
        }

        private static void rsaEncrypt(JObject request, JObject reply)
        {
            requireFields(request, "payload");
            string payload = request["payload"].ToObject<String>();
            string cert = request.ContainsKey("cert") ? request["cert"].ToObject<String>() : null;
            string key = request.ContainsKey("key") ? request["key"].ToObject<String>() : null;
            if(cert==null && key==null && !CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }

            reply["payload"]=CryptoHelper.RSAEncrypt(payload, cert, key);
        }

        private static void getCertificate(JObject request, JObject reply)
        {
            if(!CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }
            reply["payload"] = CryptoHelper.GetCertificate();

        }

        private static void getPublic(JObject request, JObject reply)
        {
            if (!CryptoHelper.CardPresentAndCertValid())
            {
                reply["status"] = "NO_CARD";
                return;
            }
            reply["payload"] = CryptoHelper.GetCert(false).PublicKey.Key.ToXmlString(false);
        }

        private static void requireFields(JObject request, params string[] fields)
        {
            foreach(var field in fields)
            {
                if(!request.ContainsKey(field))
                {
                    throw new Exception("Missing argument '" + field + "'");
                }
            }
        }

        private static void aesEncrypt(JObject request, JObject reply)
        {
            requireFields(request, "payload", "key");
            string key = request["key"].ToObject<String>();
            string payload = request["payload"].ToObject<String>();
            string ciphertext = CryptoHelper.AESEncrypt(payload, ref key);
            reply["payload"] = ciphertext;
        }

        private static void aesDecrypt(JObject request, JObject reply)
        {
            requireFields(request, "payload", "key");
            string key = request["key"].ToObject<String>();
            string payload = request["payload"].ToObject<String>();
            string ciphertext = CryptoHelper.AESDecrypt(payload, key);
            reply["payload"] = ciphertext;
        }

        private static void getAesKey(JObject request, JObject reply)
        {
            reply["payload"] = CryptoHelper.GetAESKey();
        }
    }
}
