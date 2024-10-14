namespace MyApp.Models
{
    public class LecturerClaim
    {
        public int ClaimId { get; set; }
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string AdditionalNotes { get; set; }
        public string Status { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
}
