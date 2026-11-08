namespace Project.Infrastructure.Arquivos;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string Root { get; set; } = "";

    public long MaxImageBytes { get; set; } = 5 * 1024 * 1024;

    public long MaxRequestBytes { get; set; } = 50 * 1024 * 1024;
}
