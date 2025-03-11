using Domain.Entities;

namespace Application.Dtos;

public sealed class ProfileDto
{
    public ProfileDto(User user, List<TokenDto> tokenDto)
    {
        Id        = user.Id;
        Name      = user.Name;
        Surname   = user.Surname;
        Session   = tokenDto;
        CreatedAt = user.CreateAt;
        UpdatedAt = user.UpdateAt;
    }

    public string          Id        { get; set; }
    public string          Name      { get; set; }
    public string          Surname   { get; set; }
    public List<TokenDto>  Session   { get; set; }
    public DateTimeOffset  CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}