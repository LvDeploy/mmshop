using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Abstractions.Response
{
    public class BaseResponse<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public IEnumerable<Error>? Errors { get; init; }
        public string? Message { get; init; }
        public ErrorType ErrorType { get; init; } = ErrorType.BadRequest;
    }
}
