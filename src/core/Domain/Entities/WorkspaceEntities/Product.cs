using Domain.Abstractions;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Product : WorkspaceEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}