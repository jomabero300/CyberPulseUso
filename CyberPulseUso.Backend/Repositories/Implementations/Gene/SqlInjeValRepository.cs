using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class SqlInjeValRepository : ISqlInjeValRepository
{
    private readonly string[] _sqlKeywords = { "--", ";", "'", "\"", "/*", "*/", "@@", "char", "nchar", "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "delete", "drop", "end", "exec", "execute", "fetch", "insert", "kill", "select", "sys", "sysobjects", "syscolumns", "table", "update", "union", "where", "xp_", "sp_" };
    public bool HasSqlInjection(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var lowerInput = input.ToLowerInvariant();
        foreach (var keyword in _sqlKeywords) if (lowerInput.Contains(keyword) && !IsFalsePositive(lowerInput, keyword)) return true;
        return false;
    }
    public string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        var sanitized = input.Replace("'", "").Replace("--", "").Replace(";", "").Replace("/*", "").Replace("*/", "");
        return sanitized.Length > 100 ? sanitized[..100] : sanitized;
    }
    private bool IsFalsePositive(string input, string keyword) { if (keyword.Length <= 3) return false; return false; }
}
