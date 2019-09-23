/*
 * FriendRequest.cs
 * Autor Nikola PavloviĆ
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BrzeBoljeJeftinije.Messenger.DB.Entities
{
    /**
     * <summary>Model za zahtev za prijateljstvo u bazi</summary>
     * <remarks>Verzija 1.0</remarks>
     */
    public class FriendRequest
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool Resolved { get; set; }
        public bool Seen { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
