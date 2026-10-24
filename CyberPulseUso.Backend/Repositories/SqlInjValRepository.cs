using System.Text.RegularExpressions;

namespace CyberPulseUso.Backend.Repositories;

public class SqlInjValRepository : ISqlInjValRepository
{
    private readonly string[] _sqlKeywords = {
        "--", ";", "'", "\"", "/*", "*/", "@@",
        "char", "nchar", "varchar", "nvarchar",
        "alter", "begin", "cast", "create", "cursor",
        "declare", "delete", "drop", "end", "exec",
        "execute", "fetch", "insert", "kill", "select",
        "sys", "sysobjects", "syscolumns", "table",
        "update", "union", "where", "xp_", "sp_"
    };

    private readonly Regex _sqlPatterns = new Regex(
        @"(\b(select|insert|update|delete|drop|alter|exec|execute|union)\b|--|;|'|""|/\*|\*/|@@)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public bool HasSqlInjection(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var matches = _sqlPatterns.Matches(input);

        return matches.Cast<Match>()
            .Any(match => !IsFalsePositive(input, match.Value.ToLowerInvariant()));
    }

    public string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var sanitized = _sqlPatterns.Replace(input, string.Empty);

        return sanitized.Length > 100
            ? sanitized[..100].Trim()
            : sanitized.Trim();
    }

    private bool IsFalsePositive(string input, string matchedPattern)
    {
        var lowerInput = input.ToLowerInvariant();

        return matchedPattern.ToLowerInvariant() switch
        {
            "select" => IsSelectFalsePositive(lowerInput),
            "union" => IsUnionFalsePositive(lowerInput),
            "where" => IsWhereFalsePositive(lowerInput),
            "table" => IsTableFalsePositive(lowerInput),
            "insert" => IsInsertFalsePositive(lowerInput),
            "update" => IsUpdateFalsePositive(lowerInput),
            _ => false
        };
    }

    private bool IsSelectFalsePositive(string input) => input.Contains("selective") || input.Contains("selection");
    private bool IsUnionFalsePositive(string input) => input.Contains("reunion") || input.Contains("unionized");
    private bool IsWhereFalsePositive(string input) => input.Contains("whereas") || input.Contains("wherever");
    private bool IsTableFalsePositive(string input) => input.Contains("timetable") || input.Contains("tablet");
    private bool IsInsertFalsePositive(string input) => input.Contains("insertion") || input.Contains("inserted");
    private bool IsUpdateFalsePositive(string input) => input.Contains("updated") || input.Contains("updater");
}
