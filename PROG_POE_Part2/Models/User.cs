using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // Role can be "ProgrammeCoordinator", "AcademicManager", "Lecturer", etc.
}
