namespace Project.Infrastructure.Identity;

public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";

    public bool Enabled { get; set; }

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";
}
