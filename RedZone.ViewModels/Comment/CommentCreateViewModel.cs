using System.ComponentModel.DataAnnotations;
using RedZone.Common;

namespace RedZone.ViewModels.Comment
{
    public class CommentCreateViewModel
    {
        public int MatchId { get; set; }

        [Required(ErrorMessage = ValidationConstants.Comment.ContentRequiredError)]
        [StringLength(ValidationConstants.Comment.ContentMaxLength,
            MinimumLength = ValidationConstants.Comment.ContentMinLength,
            ErrorMessage = ValidationConstants.Comment.ContentLengthError)]
        public string Content { get; set; } = null!;
    }
}