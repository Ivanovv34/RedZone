using System.ComponentModel.DataAnnotations;
using RedZone.Common;

namespace RedZone.ViewModels.Competition
{
    public class CompetitionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationConstants.Competition.NameRequiredError)]
        [StringLength(ValidationConstants.Competition.NameMaxLength,
            MinimumLength = ValidationConstants.Competition.NameMinLength,
            ErrorMessage = ValidationConstants.Competition.NameLengthError)]
        public string Name { get; set; } = null!;
    }
}