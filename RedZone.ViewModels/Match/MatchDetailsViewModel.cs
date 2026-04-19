using RedZone.ViewModels.Comment;

namespace RedZone.ViewModels.Match
{
    public class MatchDetailsViewModel
    {
        public int Id { get; set; }

        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public string CompetitionName { get; set; } = null!;

        public IEnumerable<CommentViewModel> Comments { get; set; }
            = new List<CommentViewModel>();

        public CommentCreateViewModel NewComment { get; set; }
            = new CommentCreateViewModel();
    }
}