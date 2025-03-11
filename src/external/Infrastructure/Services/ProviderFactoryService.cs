using Application.Features.Commands.Providers.v1;
using Application.Services;
using Ardalis.SmartEnum;
using Domain.Enums;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public sealed class ProviderFactoryService<TProvider> : IProviderFactoryService<TProvider> where TProvider : SmartEnum<ShippingProviderEnum>
    {
        private readonly string                      Username;
        private readonly string                      Password;
        private readonly BsonDocument? Parameters;

        public ProviderFactoryService(CreateProviderRequest request)
        {
            Username   = request.Username;
            Password   = request.Password;
        }

    }
}