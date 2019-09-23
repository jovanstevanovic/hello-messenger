/*
 * MessageCryptoMaterial.cs
 * Autor Nikola PavloviĆ
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BrzeBoljeJeftinije.Messenger.DB.Entities
{
    /**
     * <summary>Model za kriptografski materijal poruke namenjen primaocu</summary>
     * <remarks>Verzija 1.0</remarks>
     */
    public class MessageCryptoMaterial
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Material { get; set; }
    }
}
