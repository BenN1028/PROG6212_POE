using Microsoft.AspNetCore.Mvc.Rendering;

public class ClaimsViewModel
{
    public string SelectedStatus { get; set; }
    public SelectList StatusOptions { get; set; }
    public List<Claim> Claims { get; set; }

    // Property to hold the new claim being submitted
    public Claim NewClaim { get; set; } = new Claim();
}

public class Claim
{
    public int ClaimId { get; set; }
    public int HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public string AdditionalNotes { get; set; }
    public string Status { get; set; }
    public DateTime DateSubmitted { get; set; }
}

