using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record UpdateProviderRequest(
        int     ShippingProviderCode,
        string  Id,
        string? Username,
        string? Password,
        object? Parameters) : IRequest<Result<object>>;

    internal sealed record UpdateProviderHandler(
        IServiceProvider             serviceProvider) : IRequestHandler<UpdateProviderRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(UpdateProviderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory providerFactory
                = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode), serviceProvider);

            IProvider providerService = providerFactory.GetProvider();

            return await providerService.UpdateProviderAsync<object>(request, cancellationToken);
        }
    }
}