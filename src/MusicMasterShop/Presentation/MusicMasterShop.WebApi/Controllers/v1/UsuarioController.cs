using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.Application.UseCases.CreateUser;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.WebApi.Controllers.Base;

namespace MusicMasterShop.WebApi.Controllers.v1
{
    [Route("mmshop/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UsuarioController : ApiControllerBase
    {
        private IMediator _mediator;
        public UsuarioController(IMediator mediator, CorrelationId correlationId) : base(correlationId)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return CreateResponse(result);
        }
    }
}
