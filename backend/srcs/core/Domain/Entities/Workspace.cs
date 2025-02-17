using Domain.Abstractions;

namespace Domain.Entities;

public sealed class Workspace : BaseEntity {
	public string Title { get; set; }
}