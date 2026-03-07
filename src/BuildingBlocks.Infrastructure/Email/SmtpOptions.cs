namespace BuildingBlocks.Infrastructure.Email;

public class SmtpOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = true;
    public required string SenderAddress { get; set; }
    public string? SenderName { get; set; }
}
