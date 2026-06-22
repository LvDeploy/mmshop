using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.Application.Middleware.UserInfo;
using MusicMasterShop.Application.Queries.GetCart;
using MusicMasterShop.Application.UseCases.CreateCart;
using MusicMasterShop.Application.UseCases.CreateOrder;
using MusicMasterShop.Application.UseCases.UpdateCart;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.WebApi.Controllers.Base;

namespace MusicMasterShop.WebApi.Controllers.v1
{
    [Route("mmshop/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class PedidoController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserInfo _userInfo;

        public PedidoController(
            IMediator mediator,
            CorrelationId correlationId,
            IUserInfo userInfo)
            : base(correlationId)
        {
            _mediator = mediator;
            _userInfo = userInfo;
        }

        [HttpPost("criar-carrinho")]
        [ProducesResponseType(typeof(SuccessResult<CreateCartResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateCartResponse>> CreateCart(
            [FromBody] CreateCartRequest request,
            CancellationToken cancellationToken)
        {
            if (_userInfo.TipoUsuario != TipoUsuario.Vendedor)
                return Forbid();

            var result = await _mediator.Send(request, cancellationToken);
            return CreateResponse(result);
        }

        [HttpPut("atualizar-carrinho/{carrinhoId:Guid}")]
        [ProducesResponseType(typeof(SuccessResult<UpdateCartResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateCartResponse>> UpdateCart(
            Guid carrinhoId,
            [FromBody] UpdateCartRequest request,
            CancellationToken cancellationToken)
        {
            if (_userInfo.TipoUsuario != TipoUsuario.Vendedor)
                return Forbid();

            var result = await _mediator.Send(
                request with { CarrinhoId = carrinhoId },
                cancellationToken);

            return CreateResponse(result);
        }

        [HttpPost("criar-pedido")]
        [ProducesResponseType(typeof(SuccessResult<CreateOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateOrderResponse>> CreateOrder(
            [FromBody] CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            if (_userInfo.TipoUsuario != TipoUsuario.Vendedor)
                return Forbid();

            var result = await _mediator.Send(request, cancellationToken);
            return CreateResponse(result);
        }

        [HttpGet("obter-carrinho")]
        [ProducesResponseType(typeof(SuccessResult<GetCartResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(FailureResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetCartResponse>> GetCart(
            CancellationToken cancellationToken)
        {
            if (_userInfo.TipoUsuario != TipoUsuario.Vendedor)
                return Forbid();

            var result = await _mediator.Send(
                new GetCartRequest(),
                cancellationToken);

            return CreateResponse(result);
        }
    }
}
