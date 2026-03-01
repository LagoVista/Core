using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LagoVista.Models
{
    [Table("AppUser", Schema="dbo")]
    public class AppUserDTO : IEntityHeader
    {
        public string Id { get => AppUserId; set => AppUserId = value; }
        public string Text { get => FullName; set => FullName = value; }


        [MapTo("Id")]
        [Key]
        public string AppUserId { get; set; }
        [Required]
        public string Email { get; set; }
        [MapTo("Text")]
        [Required]
        public string FullName { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime LastUpdatedDate { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(AppUserId) || string.IsNullOrEmpty(FullName);
        }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(AppUserId, FullName);
        }
    }
}
