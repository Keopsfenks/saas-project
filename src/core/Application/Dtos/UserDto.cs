using Domain.Entities;

namespace Application.Dtos;

public sealed class UserDto
{
    public UserDto(User user)
    {
        Id             = user.Id;
        Name           = user.Name;
        Surname        = user.Surname;
        Email          = user.Email;
        EmailConfirmed = user.EmailConfirmed;
        CreatedAt      = user.CreateAt;
        UpdatedAt      = user.UpdateAt;
    }
    public string          Id             { get; set; }
    public string          Name           { get; set; }
    public string          Surname        { get; set; }
    public string          FullName       => $"{Name} {Surname}";
    public string          Email          { get; set; }
    public bool            EmailConfirmed { get; set; }
    public DateTimeOffset  CreatedAt      { get; set; }
    public DateTimeOffset? UpdatedAt      { get; set; }

}