using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Reflection;

namespace Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var errorDictionary = validators
                             .Select(s => s.Validate(context))
                             .SelectMany(s => s.Errors)
                             .Where(s => s != null)
                             .GroupBy(
                                  s => s.PropertyName,
                                  s => s.ErrorMessage, (propertyName, errorMessage) => new
                                  {
                                      Key = propertyName,
                                      Values = errorMessage.Distinct().ToArray()
                                  })
                             .ToDictionary(s => s.Key, s => s.Values[0]);

        if (errorDictionary.Any())
        {
            var errors = errorDictionary.Select(s => new ValidationFailure
            {
                PropertyName = s.Key,
                ErrorCode = s.Value
            });
            throw new ValidationException(errors);
        }

        return await next();
    }

    private static Dictionary<string, string> ValidateEnums(object request)
    {
        var errors = new Dictionary<string, string>();

        foreach (PropertyInfo property in request.GetType().GetProperties())
        {
            if (!property.PropertyType.IsEnum)
                continue;

            var value = property.GetValue(request);
            if (value == null || !Enum.IsDefined(property.PropertyType, value))
            {
                errors[property.Name] = $"Geçersiz {property.PropertyType.Name} değeri: {value}";
            }
        }

        return errors;
    }
}
