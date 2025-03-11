using Ardalis.SmartEnum;
using Domain.Enums;

namespace Application.Services
{
    public interface IProviderFactoryService<TProvider>
    where TProvider : SmartEnum<ShippingProviderEnum>
    {
    }
}