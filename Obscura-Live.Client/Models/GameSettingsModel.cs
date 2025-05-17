using System.ComponentModel.DataAnnotations;

namespace Obscura_Live.Client.Models
{
    public class GameSettingsModel: IValidatableObject
    {
        [Required(ErrorMessage = "Please select a start year")]
        public int YearStart { get; set; } = 2000;

        [Required (ErrorMessage = "Please select a ending year")]
        public int YearEnd { get; set; } = DateTime.Now.Year;   

        public int SelectedGenre { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           if (YearEnd <= YearStart) 
            { 
                yield return new ValidationResult("End year must be greater than start year", 
                    new[] { nameof(YearEnd) });
            }
        }
    }
}
