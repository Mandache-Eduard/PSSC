namespace Shared.Infrastructure.Results;

/// <summary>
/// Represents a result of an operation that can either succeed with a value or fail with an error.
/// This allows workflows to handle errors as values rather than exceptions.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value</typeparam>
/// <typeparam name="TError">The type of the error value</typeparam>
public abstract record Result<TSuccess, TError>
{
    /// <summary>
    /// Creates a success result
    /// </summary>
    public sealed record Success(TSuccess Value) : Result<TSuccess, TError>;

    /// <summary>
    /// Creates a failure result
    /// </summary>
    public sealed record Failure(TError Error) : Result<TSuccess, TError>;

    /// <summary>
    /// Pattern matches on the result
    /// </summary>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> onSuccess,
        Func<TError, TResult> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Value),
            Failure f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Executes an action based on the result
    /// </summary>
    public void Match(
        Action<TSuccess> onSuccess,
        Action<TError> onFailure)
    {
        switch (this)
        {
            case Success s:
                onSuccess(s.Value);
                break;
            case Failure f:
                onFailure(f.Error);
                break;
        }
    }

    /// <summary>
    /// Maps the success value to another type
    /// </summary>
    public Result<TNewSuccess, TError> Map<TNewSuccess>(
        Func<TSuccess, TNewSuccess> map) =>
        this switch
        {
            Success s => new Result<TNewSuccess, TError>.Success(map(s.Value)),
            Failure f => new Result<TNewSuccess, TError>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Binds (flatMaps) the success value to another result
    /// </summary>
    public Result<TNewSuccess, TError> Bind<TNewSuccess>(
        Func<TSuccess, Result<TNewSuccess, TError>> bind) =>
        this switch
        {
            Success s => bind(s.Value),
            Failure f => new Result<TNewSuccess, TError>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}

