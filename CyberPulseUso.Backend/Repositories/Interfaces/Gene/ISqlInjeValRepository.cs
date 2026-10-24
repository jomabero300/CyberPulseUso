namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface ISqlInjeValRepository
{
    bool HasSqlInjection(string input);
    string SanitizeInput(string input);
}
