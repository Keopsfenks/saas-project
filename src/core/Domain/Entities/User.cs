using Domain.Abstractions;

namespace Domain.Entities;

public sealed class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FullName => $"{Name} {Surname}";

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; } = false;

}