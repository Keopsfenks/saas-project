namespace Application.Dtos;

public sealed class ProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public List<TokenDto> Session { get; set; } = new();
}