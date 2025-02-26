namespace Domain.Dtos;

public sealed class ProfileDto {
	public string         Name    { get; set; }
	public string         Surname { get; set; }
	public List<TokenDto> Session { get; set; }
}