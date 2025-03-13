using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MongoDB.Bson;

namespace Application.Dtos
{
    public sealed class ProviderDto
    {
        public ProviderDto(Provider provider)
        {
            Id                   = provider.Id;
            Username             = provider.Username;
            Password             = provider.Password;
            Parameters           = provider.Parameters!.ToDictionary();
            ShippingProviderCode = provider.ShippingProvider;
            CreatedAt            = provider.CreateAt;
            UpdatedAt            = provider.UpdateAt;
        }

        public string                     Id                   { get; set; }
        public string                     Username             { get; set; }
        public string                     Password             { get; set; }
        public Dictionary<string, object> Parameters           { get; set; }
        public ShippingProviderEnum       ShippingProviderCode { get; set; }
        public DateTimeOffset             CreatedAt            { get; set; }
        public DateTimeOffset?            UpdatedAt            { get; set; }
    }
}