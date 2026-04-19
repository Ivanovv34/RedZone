namespace RedZone.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public bool CanDelete { get; set; }
    }
}