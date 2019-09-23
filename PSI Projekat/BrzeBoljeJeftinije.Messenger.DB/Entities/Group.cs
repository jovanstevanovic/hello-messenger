/*
 * Group.cs
 * Autor Nikola PavloviĆ
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BrzeBoljeJeftinije.Messenger.DB.Entities
{
    /**
     * <summary>Model za grupu u bazi</summary>
     * <remarks>Verzija 1.0</remarks>
     */
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; } //datum kreiranja
        public DateTime? LastMessage { get; set; }   //ovo ne ide u bazu, već ovde treba upisati datum poslednje poruke iz
                                                     //ove grupe, ako nema poruka ostaviti null

        public bool? IsAdmin { get; set; } // takodje ne ide u bazu, vec treba popuniti kad se radi
                                           // GetGroupsForUser, u ostalim slučajevima može da ostane null

        public bool? ContainsUnread { get; set; } // isto
        public byte[] Picture { get; set; }
        public string PictureType { get; set; }
        public bool Binary { get; set; }
    }
}
