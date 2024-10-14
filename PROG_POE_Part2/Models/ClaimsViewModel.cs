using Microsoft.AspNetCore.Mvc.Rendering;

public class ClaimsViewModel
{
    public string SelectedStatus { get; set; }
    public SelectList StatusOptions { get; set; }
    public List<LecturerClaim> Claims { get; set; }

    // Property to hold the new claim being submitted
    public LecturerClaim NewClaim { get; set; } = new LecturerClaim();
}

public class LecturerClaim
{
    public int ClaimId { get; set; }
    public int HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public string AdditionalNotes { get; set; }
    public string Status { get; set; }
    public DateTime DateSubmitted { get; set; }
}

