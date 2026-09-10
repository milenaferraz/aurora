namespace Aurora.Infrastructure.Hermes;

public record HermesOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public bool UseFake { get; init; } = true;
}
