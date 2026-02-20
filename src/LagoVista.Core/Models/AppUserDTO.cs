using LagoVista.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LagoVista.Models
{
    [Table("AppUser")]
    public class AppUserDTO
    {
        public string AppUserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(AppUserId, FullName);
        }
    }
}
