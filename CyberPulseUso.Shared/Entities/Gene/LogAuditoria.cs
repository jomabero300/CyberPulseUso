
using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

public class LogAuditoria
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(50)")]
    [Display(Name = "TableName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? TableName { get; set; }

    [Column(TypeName = "varchar(3)")]
    [Display(Name = "PrimaryKey", ResourceType = typeof(Literals))]
    [MaxLength(3, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? PrimaryKey { get; set; }

    [Column(TypeName = "char(1)")]
    [Display(Name = "Action", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? Action { get; set; }

    [Column(TypeName = "varchar(450)")]
    [Display(Name = "UserId", ResourceType = typeof(Literals))]
    [MaxLength(450, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? UserId { get; set; }

    [Column(TypeName = "varchar(250)")]
    [Display(Name = "OldValues", ResourceType = typeof(Literals))]
    [MaxLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? OldValues { get; set; }

    [Column(TypeName = "varchar(250)")]
    [Display(Name = "NewValues", ResourceType = typeof(Literals))]
    [MaxLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? NewValues { get; set; }

    [Column(TypeName = "varchar(250)")]
    [Display(Name = "AffectedColumns", ResourceType = typeof(Literals))]
    [MaxLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? AffectedColumns { get; set; }

    [Column(TypeName = "int")]
    [Display(Name = "StockActual", ResourceType = typeof(Literals))]
    public int StockActual { get; set; }

    [Column(TypeName = "datetime")]
    [Display(Name = "FechaRegistro", ResourceType = typeof(Literals))]
    public DateTime FechaRegistro { get; set; }
}
