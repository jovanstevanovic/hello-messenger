/**
 * CryptoHelper.cs
 * Autor: Nikola Pavlović
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CardCrypto
{
    /**
     * <summary>Helper klasa za kriptografske operacije</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    static class CryptoHelper
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static class NativeMethods
        {
            public const uint PROV_RSA_FULL = 0x00000001;
            public const uint CRYPT_VERIFYCONTEXT = 0xF0000000;
            public const uint CRYPT_FIRST = 0x00000001;
            public const uint CRYPT_NEXT = 0x00000002;
            public const uint ERROR_NO_MORE_ITEMS = 0x00000103;
            public const uint PP_ENUMCONTAINERS = 0x00000002;
            public const uint SCARD_E_INVALID_CHV = 0x8010002A;
            public const uint SCARD_W_WRONG_CHV = 0x8010006B;
            public const uint SCARD_W_CHV_BLOCKED = 0x8010006C;

            [DllImport("advapi32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            public static extern bool CryptAcquireContext(
            ref IntPtr phProv,
            [MarshalAs(UnmanagedType.LPStr)] string pszContainer,
            [MarshalAs(UnmanagedType.LPStr)] string pszProvider,
            uint dwProvType,
            uint dwFlags);

            [DllImport("advapi32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            public static extern bool CryptGetProvParam(
            IntPtr hProv,
            uint dwParam,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder pbData,
            ref uint pdwDataLen,
            uint dwFlags);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool CryptReleaseContext(
            IntPtr hProv,
            uint dwFlags);
        }
        private static byte[] getRandomBlock(int size = 16)
        {
            byte[] key = new byte[size];
            rngCsp.GetBytes(key);
            return key;
        }
        public static string GetAESKey()
        {
            return Convert.ToBase64String(getRandomBlock());
        }
        public static string AESEncrypt(string plaintext, ref string key)
        {
            if (key == null)
            {
                key = Convert.ToBase64String(getRandomBlock());
            }
            if(key=="sim")
            {
                return plaintext;
            }
            AesCryptoServiceProvider aesCsp = new AesCryptoServiceProvider();
            aesCsp.Key = Convert.FromBase64String(key);
            var aesIV = aesCsp.IV;
            var encryptor = aesCsp.CreateEncryptor(Convert.FromBase64String(key), aesIV);
            using (var mStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var cryptoWriter = new BinaryWriter(cryptoStream))
                    {
                        cryptoWriter.Write(Convert.FromBase64String(plaintext));
                    }
                    var ciphertext = Convert.ToBase64String(mStream.ToArray());
                    return Convert.ToBase64String(aesIV) + "$" + ciphertext;
                }
            }
        }
        public static string AESDecrypt(string encrypted, string key)
        {
            if(key=="sim")
            {
                return encrypted;
            }
            var parts = encrypted.Split('$');
            var iv = Convert.FromBase64String(parts[0]);
            var ciphetext = Convert.FromBase64String(parts[1]);
            AesCryptoServiceProvider aesCsp = new AesCryptoServiceProvider();
            aesCsp.Key = Convert.FromBase64String(key);
            aesCsp.IV = iv;
            var decryptor = aesCsp.CreateDecryptor(Convert.FromBase64String(key), iv);
            using (var mStream = new MemoryStream(ciphetext))
            {
                using (var cryptoStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                {
                    using (var outStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[16];
                        int cnt = 16;
                        while(cnt>0)
                        {
                            cnt = cryptoStream.Read(buffer, 0, 16);
                            if(cnt>0)
                            {
                                outStream.Write(buffer, 0, cnt);
                            }
                        }
                        return Convert.ToBase64String(outStream.ToArray());
                    }
                }
            }
                
        }
        public static string RSAEncrypt(string plaintext, string cert, string key)
        {
            if("sim"==cert || "sim"==key)
            {
                return plaintext;
            }
            byte[] plaintextBytes = Convert.FromBase64String(plaintext);
            RSACryptoServiceProvider csp = null;
            if(cert!=null)
            {
                csp = LoadCert(cert).PublicKey.Key as RSACryptoServiceProvider;
            }
            else if(key!=null)
            {
                csp = new RSACryptoServiceProvider();
                csp.FromXmlString(key);
            }
            else
            {
                csp = GetCert(false).PublicKey.Key as RSACryptoServiceProvider;
            }
            return Convert.ToBase64String(csp.Encrypt(plaintextBytes, false));
        }
        public static string RSADecrypt(string ciphertext)
        {
            byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);
            var provider = GetProvider(false);
            return Convert.ToBase64String(provider.Decrypt(ciphertextBytes, false));
        }
        public static string[] RSADecryptMulti(string[] ciphertext)
        {
            byte[][] ciphertextBytes = ciphertext.Select(x=>Convert.FromBase64String(x)).ToArray();
            var provider = GetProvider(false);
            return ciphertextBytes.Select(x => Convert.ToBase64String(provider.Decrypt(x, false))).ToArray();
        }
        public static string Sign(string payload)
        {
            byte[] payloadBytes = Convert.FromBase64String(payload);
            var provider = GetProvider(false);
            byte[] signature = provider.SignData(payloadBytes, CryptoConfig.MapNameToOID("SHA256"));
            return Convert.ToBase64String(signature);
        }
        public static bool Verify(string payload, string signature, string cert, string key)
        {
            if ("sim" == cert || "sim" == key)
            {
                return true;
            }
            byte[] payloadBytes = Convert.FromBase64String(payload);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            RSACryptoServiceProvider csp = null;
            if (cert != null)
            {
                csp = LoadCert(cert).PublicKey.Key as RSACryptoServiceProvider;
            }
            else if (key != null)
            {
                csp = new RSACryptoServiceProvider();
                csp.FromXmlString(key);
            }
            else
            {
                csp = GetCert(false).PublicKey.Key as RSACryptoServiceProvider;
            }
            return csp.VerifyData(payloadBytes, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);
        }
        public static List<string> GetKeyContainers(string providerName)
        {
            List<string> containers = new List<string>();

            IntPtr hProv = IntPtr.Zero;

            try
            {
                if (!NativeMethods.CryptAcquireContext(ref hProv, null, providerName, NativeMethods.PROV_RSA_FULL, NativeMethods.CRYPT_VERIFYCONTEXT))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                uint pcbData = 0;
                uint dwFlags = NativeMethods.CRYPT_FIRST;
                if (!NativeMethods.CryptGetProvParam(hProv, NativeMethods.PP_ENUMCONTAINERS, null, ref pcbData, dwFlags))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                StringBuilder sb = new StringBuilder((int)pcbData + 1);
                while (NativeMethods.CryptGetProvParam(hProv, NativeMethods.PP_ENUMCONTAINERS, sb, ref pcbData, dwFlags))
                {
                    containers.Add(sb.ToString());
                    dwFlags = NativeMethods.CRYPT_NEXT;
                }

                int err = Marshal.GetLastWin32Error();
                if (err != NativeMethods.ERROR_NO_MORE_ITEMS)
                    throw new Win32Exception(err);

                if (hProv != IntPtr.Zero)
                {
                    if (!NativeMethods.CryptReleaseContext(hProv, 0))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    hProv = IntPtr.Zero;
                }
            }
            catch
            {
                if (hProv != IntPtr.Zero)
                {
                    if (!NativeMethods.CryptReleaseContext(hProv, 0))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    hProv = IntPtr.Zero;
                }

                throw;
            }

            return containers;
        }
        public static RSACryptoServiceProvider GetProvider(bool useSigning)
        {
            List<string> containers;
            try
            {
                containers = GetKeyContainers("Microsoft Base Smart Card Crypto Provider");
            }
            catch
            {
                return null;
            }
            foreach (var container in containers)
            {
                try
                {
                    CspParameters cspParameters = new CspParameters(1, "Microsoft Base Smart Card Crypto Provider", container);
                    cspParameters.Flags = CspProviderFlags.UseExistingKey;
                    if (useSigning) cspParameters.KeyNumber = (int)KeyNumber.Signature;
                    else cspParameters.KeyNumber = (int)KeyNumber.Exchange;
                    RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParameters);
                    rsaProvider.ToXmlString(false);
                    return rsaProvider;
                }
                catch (CryptographicException)
                {
                    continue;
                }
            }
            return null;
        }
        public static X509Certificate2 GetCert(bool useSigning)
        {
            var provider = GetProvider(useSigning);
            if (provider == null) return null;
            var pubKeyXml = provider.ToXmlString(false);
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            foreach (X509Certificate2 cert in x509Store.Certificates)
            {
                if ((cert.PublicKey.Key.ToXmlString(false) == pubKeyXml) && cert.HasPrivateKey)
                    return cert;
            }
            return null;
        }
        public static String GetCertificate()
        {
            var certificate = GetCert(false);
            if (certificate == null) return null;
            return Convert.ToBase64String(certificate.Export(X509ContentType.Cert));
        }
        private static X509Certificate2[] GetMUPRSCert()
        {
            var certs = new string[] { "MUPCAGradjani.crt", "MUPCAGradjani2.crt", "MUPCAGradjani3.crt", "MUPCARoot.crt", "MUPCARoot3.crt" };
            List<X509Certificate2> returned = new List<X509Certificate2>();
            foreach (var cert in certs)
            {
                var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), cert);
                returned.Add(new X509Certificate2(File.ReadAllBytes(path)));
            }
            return returned.ToArray();
        }
        public static X509Certificate2 LoadCert(string cert)
        {
            return new X509Certificate2(Convert.FromBase64String(cert));
        }
        public static bool CertificateValid(X509Certificate2 certificateToValidate)
        {
            if (certificateToValidate == null) return false;
            if (!(certificateToValidate.PublicKey.Key is RSACryptoServiceProvider)) return false;
            X509Certificate2[] authority = GetMUPRSCert();
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.VerificationTime = DateTime.Now;

            foreach (var crt in authority)
            {
                chain.ChainPolicy.ExtraStore.Add(crt);
            }

            bool isChainValid = chain.Build(certificateToValidate);

            if (!isChainValid)
            {
                return false;
            }

            var valid = chain.ChainElements
                .Cast<X509ChainElement>()
                .Any(x => authority.Any(y => x.Certificate.Thumbprint == y.Thumbprint));

            return valid;
        }
        public static bool CardPresentAndCertValid()
        {
            X509Certificate2 certificateToValidate = GetCert(false);
            return CertificateValid(certificateToValidate);
        }
    }
}
