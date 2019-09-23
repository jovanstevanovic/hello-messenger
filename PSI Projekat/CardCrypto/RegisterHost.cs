/**
 * RegisterHost.cs
 * Autor: Nikola Pavlović
 */
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardCrypto
{
    /**
     * <summary>Klasa koja registruje native host pri instalaciji </summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    [RunInstaller(true)]
    public partial class RegisterHost : System.Configuration.Install.Installer
    {
        sealed class Options
        {
            public Options(string assemblyPath)
            {
                manifestChrome = Path.GetDirectoryName(assemblyPath) + "\\" + Path.GetFileNameWithoutExtension(assemblyPath) + "_chrome.manifest.json";
                manifestFirefox =Path.GetDirectoryName(assemblyPath) + "\\" + Path.GetFileNameWithoutExtension(assemblyPath) + "_firefox.manifest.json";
                this.assemblyPath = assemblyPath;
            }
            public string hive = "HKLM";
            public string manifestChrome;
            public string manifestFirefox;
            public string assemblyPath;
        }
        private static string eKeyChrome = "lammmgffjnohfeoiceccbmenhcjadooj";
        private static string eKeyFirefox = "messenger.cardreader@brzeboljejeftinije.rs";
        private static string eName = "brzeboljejeftinije.messenger.cardreader";
        public RegisterHost()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            int processId = Process.GetCurrentProcess().Id;
            /*string message = string.Format("Please attach the debugger (elevated on Vista or Win 7) to process [{0}].", processId);
            MessageBox.Show(message);*/
            Options options = new Options(this.Context.Parameters["assemblypath"]);
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;
                proc.Arguments += "register noadmin";
                proc.Verb = "runas";
                try
                {
                    Process.Start(proc);
                }
                catch (Exception)
                {
                    
                }
                return;
            }

            RegistryKey regkey = Registry.CurrentUser;
            RegistryKey installRegkey = Registry.LocalMachine;
            string keyNameChrome = "";
            string keyNameFirefox = "";
            string installKeyName;
            string inserted = "";
            if (Environment.Is64BitOperatingSystem)
            {
                inserted = "\\Wow6432Node";
            }

            installKeyName = "HKEY_LOCAL_MACHINE\\Software" + inserted + "\\Google\\Chrome\\Extensions\\";

            keyNameChrome = "Software" + inserted + "\\Google\\Chrome\\NativeMessagingHosts\\";
            keyNameFirefox = "Software\\Mozilla\\NativeMessagingHosts\\";
            regkey = Registry.LocalMachine;


            try
            {
                StreamWriter manifest = File.CreateText(options.manifestChrome);
                manifest.Write(new JObject(
                        new JProperty("name", eName),
                        new JProperty("description", "Serbian ID Card Reader"),
                        new JProperty("type", "stdio"),
                        new JProperty("path", options.assemblyPath),
                        new JProperty("allowed_origins",
                            new JArray(
                                new JValue(string.Format("chrome-extension://{0}/", eKeyChrome))
                                )
                            )
                    ).ToString()
                    );
                manifest.Close();
                manifest = File.CreateText(options.manifestFirefox);
                manifest.Write(new JObject(
                        new JProperty("name", eName),
                        new JProperty("description", "Eurobank card reader plugin"),
                        new JProperty("type", "stdio"),
                        new JProperty("path", options.assemblyPath),
                        new JProperty("allowed_extensions",
                            new JArray(
                                new JValue(eKeyFirefox)
                                )
                            )
                    ).ToString()
                    );
                manifest.Close();
            }
            catch (Exception ex)
            {
                return;
            }

            try
            {
                regkey = regkey.CreateSubKey(keyNameChrome + eName);
                regkey.SetValue(null, options.manifestChrome);

                var baseReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, string.IsNullOrEmpty(inserted) ? RegistryView.Default : RegistryView.Registry64);
                regkey = baseReg;
                regkey = regkey.CreateSubKey(keyNameFirefox + eName);
                regkey.SetValue(null, options.manifestFirefox);
            }
            catch (Exception ex)
            {
                Console.ReadKey();
                return;
            }
            return;
        }
    }
}
