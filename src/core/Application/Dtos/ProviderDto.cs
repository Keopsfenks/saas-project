using Domain.Entities.WorkspaceEntities;
using Domain.Enums;

namespace Application.Dtos
{
    public sealed class ProviderDto
    {
        public ProviderDto(Provider provider)
        {
            Id                   = provider.Id;
            Username             = provider.Username;
            Password             = provider.Password;
            ShippingProviderCode = provider.ShippingProvider;
        }
        public string               Id                   { get; set; }
        public string               Username             { get; set; }
        public string               Password             { get; set; }
        public ShippingProviderEnum ShippingProviderCode { get; set; }
    }
}