using Domain.Abstractions;

namespace Domain.Entities;

public sealed class User : BaseEntity {
	public string Name     { get; set; }
	public string Surname  { get; set; }
	public string FullName => $"{Name} {Surname}";

	public string Email          { get; set; }
	public string Password       { get; set; }
	public bool   EmailConfirmed { get; set; } = false;

}