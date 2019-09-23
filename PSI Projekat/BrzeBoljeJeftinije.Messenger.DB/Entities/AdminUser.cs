/*
 * AdminUser.cs
 * Autor Nikola PavloviĆ
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BrzeBoljeJeftinije.Messenger.DB.Entities
{
    /**
     * <summary>Model za Admin korisnika u bazi</summary>
     * <remarks>Verzija 1.0</remarks>
     */
    public class AdminUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void SetPassword(string password)
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            random.GetBytes(salt);
            Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] toStore = pbkdf.GetBytes(32);
            this.Password = Convert.ToBase64String(salt)+"$"+Convert.ToBase64String(toStore);
        }

        public bool ValidatePassword(string password)
        {
            if (this.Password == "$") return true;
            var salt = Convert.FromBase64String(this.Password.Split('$')[0]);
            var key = this.Password.Split('$')[1];
            Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] toStore = pbkdf.GetBytes(32);
            return Convert.ToBase64String(toStore) == key;
        }
    }
}
