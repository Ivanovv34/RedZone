namespace RedZone.ViewModels.Match
{
    public class MatchIndexPageViewModel
    {
        public IEnumerable<MatchIndexViewModel> Matches { get; set; }
            = new List<MatchIndexViewModel>();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage => this.CurrentPage > 1;

        public bool HasNextPage => this.CurrentPage < this.TotalPages;
    }
}