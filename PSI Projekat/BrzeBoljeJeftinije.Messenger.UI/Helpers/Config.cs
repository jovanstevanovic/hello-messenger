/**
 * Config.cs
 * Autor: Nikola Pavlović
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace BrzeBoljeJeftinije.Messenger.UI.Helpers
{
    /**
     * <summary>Helper klasa za pristup konfiguracionim parametrima iz web.config</summary>
     */
    public static class Config
    {
        public static bool SimLoginEnabled
        {
            get
            {
                return ConfigurationManager.AppSettings["SimLoginEnabled"] == "true";
            }
        }
    }
}