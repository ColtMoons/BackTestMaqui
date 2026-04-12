using Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Behaviors;

public partial class LoggingBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string name = request.GetType().Name;

        LogExecutingCommand(name);

        TResponse result = await next(cancellationToken);

        LogCommandProcessedSuccessfully(name);

        return result;
    }

    [LoggerMessage(LogLevel.Information, "Executing command {Command}")]
    partial void LogExecutingCommand(string command);

    [LoggerMessage(LogLevel.Information, "Command {Command} processed successfully")]
    partial void LogCommandProcessedSuccessfully(string command);
}
