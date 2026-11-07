namespace CyberPulseUso.Shared.EntitiesDTO.Gene;

public class TokenDTO
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public string? RefreshToken { get; set; }
    public bool RememberMe { get; set; }
}
