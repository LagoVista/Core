using LagoVista.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LagoVista.Models
{
    [Table("AppUser", Schema="dbo")]
    public class AppUserDTO
    {
        [Key]
        public string AppUserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastUpdatedDate { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(AppUserId, FullName);
        }
    }
}
