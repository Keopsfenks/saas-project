namespace Domain.Abstractions;

public interface IEntity {
	string                 Id        { get; set; }
	bool                   IsDeleted { get; set; }
	public DateTimeOffset? UpdateAt  { get; set; }
	public DateTimeOffset  CreateAt  { get; set; }

}