using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using RedZone.Common;

namespace RedZone.Data.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.Notification.MessageMaxLength)]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;
    }
}