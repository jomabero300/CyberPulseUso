using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa;

public class LaborRemunerationParametersValuePivotDTO
{
    public int LRPId { get; set; }

    [Column("Groupo VI")]
    public decimal? GroupVI { get; set; }

    [Column("Groupo III")]
    public decimal? GroupIII { get; set; }

    [Column("Groupo II")]
    public decimal? GroupII { get; set; }

    [Column("Groupo I")]
    public decimal? GroupI { get; set; }

    [Column("Groupo 8")]
    public decimal? Group8 { get; set; }

    [Column("Groupo 7")]
    public decimal? Group7 { get; set; }

    [Column("Groupo 6")]
    public decimal? Group6 { get; set; }

    [Column("Groupo 5")]
    public decimal? Group5 { get; set; }

    [Column("Groupo 4")]
    public decimal? Group4 { get; set; }

    [Column("Groupo 3")]
    public decimal? Group3 { get; set; }

    [Column("Groupo 2A")]
    public decimal? Group2A { get; set; }

    [Column("Groupo 2")]
    public decimal? Group2 { get; set; }

    [Column("Groupo 1")]
    public decimal? Group1 { get; set; }
}