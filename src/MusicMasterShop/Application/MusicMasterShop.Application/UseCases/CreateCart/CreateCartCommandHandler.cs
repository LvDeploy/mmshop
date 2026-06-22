using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Middleware.UserInfo;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateCart;

public sealed class CreateCartCommandHandler
    : IRequestHandler<CreateCartRequest, BaseResponse<CreateCartResponse>>
{
    private readonly IUserInfo _userInfo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public CreateCartCommandHandler(
        IUserInfo userInfo,
        IUnitOfWork unitOfWork,
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _userInfo = userInfo;
        _unitOfWork = unitOfWork;
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<BaseResponse<CreateCartResponse>> Handle(
        CreateCartRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return ResponseWrapper.Failure<CreateCartResponse>(
                request.ValidationResult.Errors,
                ErrorType.BadRequest);
        }

        if (!_userInfo.IsAuthenticated
            || _userInfo.TipoUsuario != TipoUsuario.Vendedor
            || !_userInfo.Id.HasValue)
        {
            return ResponseWrapper.Failure<CreateCartResponse>(
                Error.Set("Apenas usuários vendedores podem criar carrinhos"),
                ErrorType.Forbidden);
        }

        Produto? produto = await _produtoRepository.Get(
            request.ProdutoId,
            cancellationToken);

        if (produto is null)
        {
            return ResponseWrapper.Failure<CreateCartResponse>(
                Error.Set("Produto não encontrado"),
                ErrorType.NotFound);
        }

        Usuario? usuario = await _usuarioRepository.Get(_userInfo.Id.Value, cancellationToken);

        if (usuario is null || !usuario.Ativo)
        {
            return ResponseWrapper.Failure<CreateCartResponse>(
                Error.Set("Usuário não encontrado ou inativo"),
                ErrorType.NotFound);
        }

        Carrinho? carrinho = await _carrinhoRepository.GetActiveByUsuarioIdAsync(
            _userInfo.Id.Value,
            cancellationToken);

        if (carrinho is null)
        {
            carrinho = Carrinho.Create(request.Quantidade, produto, usuario);
            _carrinhoRepository.Create(carrinho);
        }
        else
        {
            carrinho.AddOrUpdateProduto(produto, request.Quantidade);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return ResponseWrapper.Success(new CreateCartResponse(carrinho.Id));
    }
}
