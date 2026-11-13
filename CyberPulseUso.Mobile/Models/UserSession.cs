namespace CyberPulseUso.Mobile.Models;

public class UserSession
{
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Photo { get; set; } = string.Empty;

    public string UserType { get; set; } = string.Empty;

    public string ChargeName { get; set; } = string.Empty;

    public int CountryId { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public ImageSource? PhotoImage { get; set; }
}