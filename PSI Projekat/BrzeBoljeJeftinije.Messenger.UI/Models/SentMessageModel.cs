/**
 * SentMessageModel.cs
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
     * <summary>ViewModel za operaciju slanja poruke</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class SentMessageModel
    {
        [Required]
        public int? GroupId { get; set; }

        [Required]
        public string Ciphertext { get; set; }

        [Required]
        public string Materials { get; set; }

        public string AttachmentName { get; set; }
        public string AttachmentType { get; set; }
        public string Attachment { get; set; }
    }
}