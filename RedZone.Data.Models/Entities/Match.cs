using System.ComponentModel.DataAnnotations;
using RedZone.Data.Models.Enums;

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

        public MatchStatus Status { get; set; } = MatchStatus.Upcoming;

        public int CompetitionId { get; set; }

        public Competition Competition { get; set; } = null!;

        public MatchResult? Result { get; set; }

        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}