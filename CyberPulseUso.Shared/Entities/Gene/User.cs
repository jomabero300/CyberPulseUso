using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

public class User : IdentityUser
{
    [Column(TypeName = "varchar(12)")]
    [Display(Name = "DocumentId", ResourceType = typeof(Literals))]
    [MaxLength(12, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string DocumentId { get; set; } = null!;

    [Column(TypeName = "varchar(50)")]
    [Display(Name = "FirstName", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [RegularExpression(
        @"^[a-zA-Z0-9Ã¡Ã©Ã­Ã³ÃºÃÃ‰ÃÃ“ÃšÃ±Ã‘\s]*$",
        ErrorMessageResourceName = "RegularExpression",
        ErrorMessageResourceType = typeof(Literals))]
    public string FirstName { get; set; } = null!;

    [Column(TypeName = "varchar(50)")]
    [Display(Name = "LastName", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [RegularExpression(
        @"^[a-zA-Z0-9Ã¡Ã©Ã­Ã³ÃºÃÃ‰ÃÃ“ÃšÃ±Ã‘\s]*$",
        ErrorMessageResourceName = "RegularExpression",
        ErrorMessageResourceType = typeof(Literals))]
    public string LastName { get; set; } = null!;

    [Column(TypeName = "varchar(100)")]
    [Display(Name = "Image", ResourceType = typeof(Literals))]
    public string? Photo { get; set; }

    [Display(Name = "UserType", ResourceType = typeof(Literals))]
    public UserType UserType { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    [Range(
        1,
        int.MaxValue,
        ErrorMessageResourceName = "RequiredField",
        ErrorMessageResourceType = typeof(Literals))]
    [Required(
        ErrorMessageResourceName = "RequiredField",
        ErrorMessageResourceType = typeof(Literals))]
    public int CountryId { get; set; }

    [Display(Name = "User", ResourceType = typeof(Literals))]
    public string FullName => $"{FirstName} {LastName}";

    public string PhotoFull =>
        string.IsNullOrEmpty(Photo)
            ? "/Images/NoImage.png"
            : Photo;

    public Country Country { get; set; } = null!;
}
