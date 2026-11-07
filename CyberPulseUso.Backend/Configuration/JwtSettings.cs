namespace CyberPulseUso.Backend.Configuration;

public class JwtSettings
{
    public int AccessTokenMinutes { get; set; } = 15;

    public int NormalSessionHours { get; set; } = 5;

    public int RefreshTokenDays { get; set; } = 7;
}