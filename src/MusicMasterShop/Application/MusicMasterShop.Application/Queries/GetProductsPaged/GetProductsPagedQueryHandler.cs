using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Queries.GetProduct;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Pagination;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.GetProductsPaged;

public sealed class GetProductsPagedQueryHandler
    : IRequestHandler<GetProductsPagedRequest, BaseResponse<PagedResult<GetProductResponse>>>
{
    private readonly IProdutoRepository _produtoRepository;

    public GetProductsPagedQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<BaseResponse<PagedResult<GetProductResponse>>> Handle(
        GetProductsPagedRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return ResponseWrapper.Failure<PagedResult<GetProductResponse>>(
                request.ValidationResult.Errors,
                ErrorType.BadRequest);
        }

        PagedResult<Produto> products = await _produtoRepository.GetAllPagedWithDetailsAsync(
            request.PageSize,
            request.PageNumber,
            cancellationToken);

        var response = new PagedResult<GetProductResponse>
        {
            Items = products.Items.Select(Map).ToList(),
            CurrentPage = products.CurrentPage,
            PageSize = products.PageSize,
            TotalCount = products.TotalCount
        };

        return ResponseWrapper.Success(response);
    }

    private static GetProductResponse Map(Produto produto)
    {
        return new GetProductResponse(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Modelo,
            produto.Marca,
            produto.SerialNumber,
            produto.GarantiaEmDias,
            produto.Preco,
            produto.QtdDisponivel,
            produto.Categoria.Tipo,
            produto.Dimensao);
    }
}
