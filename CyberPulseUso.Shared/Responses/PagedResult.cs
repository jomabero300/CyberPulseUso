namespace CyberPulseUso.Shared.Responses;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalRecords { get; init; }}
