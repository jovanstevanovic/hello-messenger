/**
 * AdminUser.cs
 * Autor: Nikola Pavlović
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BrzeBoljeJeftinije.Messenger.UI.Models
{
    /**
     * <summary>ViewModel za login formu za administratora</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class AdminUser
    {
        [DisplayName("Korisničko ime")]
        public string Username { get; set; }
        [DisplayName("Šifra")]
        public string Password { get; set; }
    }
}