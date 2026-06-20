using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.Application.UseCases.Login;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.WebApi.Controllers.Base;

namespace MusicMasterShop.WebApi.Controllers.v1
{
    [Route("mmshop/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator, CorrelationId correlationId) : base(correlationId)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuccessResult<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return CreateResponse(result);
        }
    }
}
