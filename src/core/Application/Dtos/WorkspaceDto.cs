using Domain.Entities;

namespace Application.Dtos;

public sealed class WorkspaceDto
{
    public WorkspaceDto(Workspace workspace)
    {
        Id = workspace.Id;
        Title = workspace.Title;
        Description = workspace.Description;
        CreatedAt = workspace.CreateAt;
        UpdatedAt = workspace.UpdateAt;
    }
    public string         Id          { get; set; }
    public string         Title       { get; set; }
    public string         Description { get; set; }
    public DateTimeOffset CreatedAt   { get; set; }
    public DateTimeOffset? UpdatedAt   { get; set; }
}