namespace abcsxl.Services.Authentication
{
    public sealed record AuthenticatedUser(
        Guid Id,
        string Username,
        string Email,
        string Role,
        string? DisplayName);
}
