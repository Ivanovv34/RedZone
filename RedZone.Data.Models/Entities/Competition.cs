using System.ComponentModel.DataAnnotations;
using RedZone.Common;

namespace RedZone.Data.Models.Entities
{
    public class Competition
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.Competition.NameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}