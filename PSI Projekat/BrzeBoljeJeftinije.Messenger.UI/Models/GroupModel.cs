/**
 * GroupModel.cs
 * Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace BrzeBoljeJeftinije.Messenger.UI.Models
{
    /**
     * <summary>ViewModel za operacije editovanja grupa</summary>
     * 
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public class GroupModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage ="Naziv grupe je obavezno polje")]
        public string Name { get; set; }

        [Required]
        public string Members { get; set; }

        public HttpPostedFileBase Picture { get; set; }

        private List<User> members;

        public List<User> GetMembers()
        {
            if(string.IsNullOrWhiteSpace(Members))
            {
                return new List<User>();
            }
            members=Json.Decode<List<User>>(Members);
            return members;
        }
        public void SetMembers(List<User> members)
        {
            this.members = members;
            Members = Json.Encode(members.Select(x=>new { id=x.Id, name=x.Name, picture="/User/Picture/"+x.Id }));
        }
    }
}