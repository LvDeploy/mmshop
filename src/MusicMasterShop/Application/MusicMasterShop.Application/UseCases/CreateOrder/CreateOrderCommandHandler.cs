using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Middleware.UserInfo;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Application.UseCases.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderRequest, BaseResponse<CreateOrderResponse>>
{
    private readonly IUserInfo _userInfo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public CreateOrderCommandHandler(
        IUserInfo userInfo,
        IUnitOfWork unitOfWork,
        ICarrinhoRepository carrinhoRepository,
        IPedidoRepository pedidoRepository)
    {
        _userInfo = userInfo;
        _unitOfWork = unitOfWork;
        _carrinhoRepository = carrinhoRepository;
        _pedidoRepository = pedidoRepository;
    }

    public async Task<BaseResponse<CreateOrderResponse>> Handle(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return ResponseWrapper.Failure<CreateOrderResponse>(
                request.ValidationResult.Errors,
                ErrorType.BadRequest);
        }

        if (!_userInfo.IsAuthenticated
            || _userInfo.TipoUsuario != TipoUsuario.Vendedor
            || _userInfo.Id is not Guid usuarioId)
        {
            return ResponseWrapper.Failure<CreateOrderResponse>(
                Error.Set("Apenas usuários vendedores podem criar pedidos"),
                ErrorType.Forbidden);
        }

        Carrinho? carrinho = await _carrinhoRepository.GetWithProductsAsync(
            request.CarrinhoId,
            cancellationToken);

        if (carrinho is null)
        {
            return ResponseWrapper.Failure<CreateOrderResponse>(
                Error.Set("Carrinho não encontrado"),
                ErrorType.NotFound);
        }

        if (carrinho.UsuarioId != usuarioId)
        {
            return ResponseWrapper.Failure<CreateOrderResponse>(
                Error.Set("O carrinho não pertence ao usuário autenticado"),
                ErrorType.Forbidden);
        }

        if (!carrinho.Ativo)
        {
            return ResponseWrapper.Failure<CreateOrderResponse>(
                Error.Set("Carrinho está inativo"),
                ErrorType.BadRequest);
        }

        if (carrinho.Produtos.Count == 0)
        {
            return ResponseWrapper.Failure<CreateOrderResponse>(
                Error.Set("Carrinho não possui produtos"),
                ErrorType.BadRequest);
        }

        foreach (CarrinhoProduto item in carrinho.Produtos)
        {
            if (item.Quantidade <= 0
                || item.Produto.QtdDisponivel < (uint)item.Quantidade)
            {
                return ResponseWrapper.Failure<CreateOrderResponse>(
                    Error.Set($"{item.Produto.Nome} não tem estoque suficiente"),
                    ErrorType.BadRequest);
            }
        }

        foreach (CarrinhoProduto item in carrinho.Produtos)
        {
            item.Produto.RemoveQtdDisponivel((uint)item.Quantidade);
        }

        carrinho.Finalizar();

        Pedido pedido = Pedido.Create(
            request.DocumentoCliente,
            carrinho,
            StatusPedido.Validado);

        _pedidoRepository.Create(pedido);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ResponseWrapper.Success(new CreateOrderResponse(pedido.Id));
    }
}
