using System.Reflection;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Abstractions.Behaviors;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(request, validators);

        if (validationFailures.Length == 0)
        {
            return await next(cancellationToken);
        }

        return CreateValidationResult<TResponse>(validationFailures);
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        IValidator<TCommand>[] enumerable = validators as IValidator<TCommand>[] ?? validators.ToArray();
        if (!enumerable.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            enumerable.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = [.. validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)];

        return validationFailures;
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());

    private static TResult CreateValidationResult<TResult>(ValidationFailure[] validationFailures)
        where TResult : Result
    {
        ValidationError validationError = CreateValidationError(validationFailures);

        if (typeof(TResult) == typeof(Result))
        {
            return (Result.Failure(validationError) as TResult)!;
        }

        // Obtener el tipo genérico (ej: Guid en Result<Guid>)
        Type resultType = typeof(TResult).GenericTypeArguments[0];
        
        MethodInfo failureMethod = typeof(Result)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => 
                m is { Name: nameof(Result.Failure), IsGenericMethod: true } &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == typeof(Error)) ?? throw new InvalidOperationException(
                "No se pudo encontrar el método genérico Failure<TValue>(Error) en la clase Result");
       
        MethodInfo genericFailureMethod = failureMethod.MakeGenericMethod(resultType);
        
        object? validationResult = genericFailureMethod.Invoke(null, [validationError]);


        return (TResult)validationResult;
    }
}
