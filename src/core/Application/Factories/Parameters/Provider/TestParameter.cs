using Application.Factories.Interfaces;
using Domain.Enums;

namespace Application.Factories.Parameters.Provider
{
    public sealed record TestParameterProvider(
        string ApiKey,
        string ApiSecret) : IProvider;
}