namespace abcsxl.Services.Authentication
{
    public class AuthResult
    {
        public bool Succeeded { get; init; }
        public string? ErrorMessage { get; init; }
        public AuthenticatedUser? User { get; init; }

        public static AuthResult Success(AuthenticatedUser user) =>
            new() { Succeeded = true, User = user };

        public static AuthResult Failure(string errorMessage) =>
            new() { Succeeded = false, ErrorMessage = errorMessage };
    }
}
