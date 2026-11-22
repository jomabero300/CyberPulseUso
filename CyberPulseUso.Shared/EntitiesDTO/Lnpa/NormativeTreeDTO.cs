namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa;

public class NormativeTreeDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public ICollection<NormativeChapterDTO> Chapters { get; set; }
        = new List<NormativeChapterDTO>();
}