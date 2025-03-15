namespace Application.Factories.Parameters.Requests
{
    public sealed record MNGRequestToken(
        string CustomerNumber,
        string Password,
        int    identityType = 1);
}