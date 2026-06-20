using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace MusicMasterShop.WebApi.Controllers.Base
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [EnableCors("CORSPOLICY")]
    public class ApiControllerBase(CorrelationId correlationId) : ControllerBase
    {
        public ActionResult<T> CreateResponse<T>(BaseResponse<T> result)
        {
            if (result.IsSuccess)
            {
                SuccessResult<T> resultSuccess = new(correlationId.Get(), result.Data, result.Message);
                return Ok(resultSuccess);
            }

            FailureResult resultFailure = new(correlationId.Get(), [.. result.Errors!]);

            return result.ErrorType switch
            {
                ErrorType.BadRequest => BadRequest(resultFailure),
                ErrorType.Unauthorized => Unauthorized(),
                ErrorType.Forbidden => Forbid(),
                ErrorType.NotFound => NotFound(resultFailure),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
