namespace WebUI.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string? IpAddress { get; }
        string? Language { get; }
        string? Email { get; }
        string? Token { get; }
        string? MobileAppType { get; }
    }
}
