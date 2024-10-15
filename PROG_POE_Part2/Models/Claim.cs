namespace MyApp.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string Notes { get; set; }

        // Add the Status property to track the claim's status
        public string Status { get; set; }  // "Pending", "Approved", "Rejected"
    }

}
