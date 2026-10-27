namespace CyberPulseUso.Frontend.Repositories;

public interface ISqlInjValRepository
{
    bool HasSqlInjection(string input);
    string SanitizeInput(string input);
}
