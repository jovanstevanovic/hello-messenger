/**
 * LoginModel.cs
 * Autor: Nikola Pavlović
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BrzeBoljeJeftinije.Messenger.UI.Models
{
    /**
     * <summary>ViewModel za operaciju prijave korisnika</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class LoginModel
    {
        [Required]
        public string Certificate { get; set; }
        
        [Required]
        public string Signature { get; set; }
    }
}