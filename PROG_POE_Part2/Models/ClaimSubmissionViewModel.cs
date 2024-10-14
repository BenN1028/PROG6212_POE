using System.ComponentModel.DataAnnotations;

public class ClaimSubmissionViewModel
{
    [Required]
    public int HoursWorked { get; set; }

    [Required]
    public decimal HourlyRate { get; set; }

    public string AdditionalNotes { get; set; }
}
