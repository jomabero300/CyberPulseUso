using CyberPulseUso.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

    public string UserTypeDisplay
    {
        get
        {
            if (!Enum.TryParse<UserType>(UserType, true, out var userType))
                return UserType;

            var display = typeof(UserType)
                .GetMember(userType.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>();

            return display?.GetName() ?? UserType;
        }
    }
}