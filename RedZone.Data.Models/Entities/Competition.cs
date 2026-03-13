using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RedZone.Data.Models.Entities
{
    public class Competition
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}