namespace CyberPulseUso.Backend.Repositories;

public interface ISqlInjValRepository
{
    bool HasSqlInjection(string input);
    string SanitizeInput(string input);
}
