using Application.Factories;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;

namespace Application.Dtos
{
    public sealed class ProviderDto<T> where T : class
    {
        public ProviderDto(Provider provider)
        {
            Id                   = provider.Id;
            Username             = provider.Username;
            Password             = provider.Password;
            Parameters           = ParametersFactory.Deserialize<T>(provider.Parameters);
            ShippingProviderCode = ShippingProviderEnum.FromValue(provider.ShippingProvider);
            CreatedAt            = provider.CreateAt;
            UpdatedAt            = provider.UpdateAt;
        }

        public string               Id                   { get; set; }
        public string               Username             { get; set; }
        public string               Password             { get; set; }
        public T?                   Parameters           { get; set; }
        public ShippingProviderEnum ShippingProviderCode { get; set; }

        public DateTimeOffset       CreatedAt            { get; set; }
        public DateTimeOffset?      UpdatedAt            { get; set; }
    }
}