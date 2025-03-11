using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Provider : WorkspaceEntity
    {
        public string                       Username         { get; set; } = string.Empty;
        public string                       Password         { get; set; } = string.Empty;
        public ShippingProviderEnum         ShippingProvider { get; set; } = ShippingProviderEnum.None;
        public Dictionary<string, string>? Parameters       { get; set; } = null;
    }
}