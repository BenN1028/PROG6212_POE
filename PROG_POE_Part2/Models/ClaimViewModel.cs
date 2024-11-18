public class ClaimViewModel
{
    public int ClaimId { get; set; }
    public string LecturerName { get; set; }
    public int HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime DateSubmitted { get; set; }
    public string Notes { get; set; }
    public string Status { get; set; }
    public string DocumentPath { get; set; }
    public string RejectionReason { get; set; }
}
