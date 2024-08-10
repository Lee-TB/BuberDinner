using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace BuberDinner.Application.Common.Behaviors;
public class ValidateBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator;

    public ValidateBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (_validator is null)
        {
            return await next();
        }

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next();
        }

        List<Error> errors = validationResult.Errors
            .ConvertAll(validationFailure => Error.Validation(
                code: validationFailure.PropertyName,
                description: validationFailure.ErrorMessage
            ));

        return (dynamic)errors;
    }
}
