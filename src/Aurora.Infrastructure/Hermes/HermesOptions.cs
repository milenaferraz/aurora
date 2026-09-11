namespace Aurora.Infrastructure.Hermes;

public record HermesOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "hermes";
    public bool UseFake { get; init; } = true;
}
