/*
 * User.cs
 * Autor Nikola PavloviĆ
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BrzeBoljeJeftinije.Messenger.DB.Entities
{
    /**
     * <summary>Model za običnog korisnika u bazi</summary>
     * <remarks>Verzija 1.0</remarks>
     */
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CertHash { get; set; }
        public byte[] Certificate { get; set; }
        public byte[] Picture { get; set; }
        public string PictureType { get; set; }
        public string RtID { get; set; }
        public DateTime? BannedUntil { get; set; }
    }
}
