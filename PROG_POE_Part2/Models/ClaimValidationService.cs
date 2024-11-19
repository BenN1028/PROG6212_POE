public class ClaimValidationService
{
    private readonly decimal _maxHourlyRate = 100m; // Max rate per hour
    private readonly int _maxHoursPerClaim = 40; // Max hours worked per claim

    public bool ValidateClaim(Claim claim)
    {
        bool isValid = true;

        // Validate hours worked
        if (claim.HoursWorked > _maxHoursPerClaim)
        {
            claim.RejectionReason = "Exceeded maximum allowed hours.";
            isValid = false;
        }

        // Validate hourly rate
        if (claim.HourlyRate > _maxHourlyRate)
        {
            claim.RejectionReason = "Hourly rate exceeds allowed limit.";
            isValid = false;
        }

        // Additional validations can go here

        return isValid;
    }
}
