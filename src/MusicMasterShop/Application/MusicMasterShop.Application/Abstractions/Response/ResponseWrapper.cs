using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Abstractions.Response
{
    public static class ResponseWrapper
    {
        public static BaseResponse<T> Success<T>(T? data, string? message = null)
        {
            return new BaseResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static BaseResponse<T> Failure<T>(Error error, ErrorType errorType = ErrorType.BadRequest)
        {
            return Failure<T>(new[] { error }, errorType);
        }

        public static BaseResponse<T> Failure<T>(IEnumerable<Error>? errors, ErrorType errorType = ErrorType.BadRequest)
        {
            return new BaseResponse<T>
            {
                IsSuccess = false,
                Errors = errors?.ToArray() ?? Array.Empty<Error>(),
                ErrorType = errorType
            };
        }

        public static BaseResponse<T> Failure<T>(System.Collections.IEnumerable? errors, ErrorType errorType = ErrorType.BadRequest)
        {
            return Failure<T>(
                errors?.Cast<object>().Select(MapError) ?? Array.Empty<Error>(),
                errorType);
        }

        public static BaseResponse<T> Failure<T, TSource>(BaseResponse<TSource> result)
        {
            return Failure<T>(result.Errors, result.ErrorType);
        }

        private static Error MapError(object value)
        {
            if (value is Error error)
            {
                return error;
            }

            string? message = value.GetType().GetProperty("ErrorMessage")?.GetValue(value)?.ToString();
            return Error.Set(message ?? value.ToString());
        }
    }
}
