using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.Application.Queries.GetProduct;
using MusicMasterShop.Application.UseCases.CreateProduct;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.WebApi.Controllers.Base;

namespace MusicMasterShop.WebApi.Controllers.v1
{
    [Route("mmshop/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ProdutoController : ApiControllerBase
    {
        private IMediator _mediator;
        public ProdutoController(IMediator mediator, CorrelationId correlationId) : base(correlationId)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateProductResponse>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return CreateResponse(result);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(SuccessResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetProductResponse>> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var request = new GetProductRequest(id);
            var result = await _mediator.Send(request, cancellationToken);
            return CreateResponse(result);
        }
    }
}
