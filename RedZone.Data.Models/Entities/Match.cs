using System.ComponentModel.DataAnnotations;

namespace RedZone.Data.Models.Entities
{
    public class Match
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string HomeTeam { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public int CompetitionId { get; set; }

        public Competition Competition { get; set; } = null!;

        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    }
}