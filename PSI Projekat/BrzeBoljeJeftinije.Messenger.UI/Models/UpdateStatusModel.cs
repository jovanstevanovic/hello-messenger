/**
 * UpdateStatusModel.cs
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
     * <summary>ViewModel za operaciju promene stanja korisnika na admin stranici</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class UpdateStatusModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [RegularExpression("(?:ok)|(?:ban)")]
        public string Status { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}