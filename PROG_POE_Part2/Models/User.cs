using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string ContactInfo { get; set; }
}
