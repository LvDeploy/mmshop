using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Core.Result;

namespace MusicMasterShop.Test.UnitTests.Builders.Response;

public sealed class ResponseBuilder<T>
{
    private bool _isSuccess = true;
    private T? _data;
    private ErrorType _errorType = ErrorType.BadRequest;
    private IEnumerable<Error> _errors = [];
    private string? _message;

    public ResponseBuilder<T> WithData(T data)
    {
        _data = data;
        return this;
    }

    public ResponseBuilder<T> WithMessage(string value)
    {
        _message = value;
        return this;
    }

    public ResponseBuilder<T> AsFailure(ErrorType errorType, string detail = "Falha")
    {
        _isSuccess = false;
        _errorType = errorType;
        _errors = [Error.Set(detail)];
        return this;
    }

    public BaseResponse<T> Build() => new()
    {
        IsSuccess = _isSuccess,
        Data = _data,
        ErrorType = _errorType,
        Errors = _errors,
        Message = _message
    };
}
