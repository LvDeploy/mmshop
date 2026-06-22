using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Pagination;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.ListProductsPaged;

public sealed class ListProductsPagedQueryHandler
    : IRequestHandler<ListProductsPagedRequest, BaseResponse<PagedResult<ListProductsPagedResponse>>>
{
    private readonly IProdutoRepository _produtoRepository;

    public ListProductsPagedQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<BaseResponse<PagedResult<ListProductsPagedResponse>>> Handle(
        ListProductsPagedRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return ResponseWrapper.Failure<PagedResult<ListProductsPagedResponse>>(
                request.ValidationResult.Errors,
                ErrorType.BadRequest);
        }

        PagedResult<Produto> products = await _produtoRepository.GetAllPagedWithDetailsAsync(
            request.PageSize,
            request.PageNumber,
            cancellationToken);

        var response = new PagedResult<ListProductsPagedResponse>
        {
            Items = products.Items.Select(Mapping).ToList(),
            CurrentPage = products.CurrentPage,
            PageSize = products.PageSize,
            TotalCount = products.TotalCount
        };

        return ResponseWrapper.Success(response);
    }

    private static ListProductsPagedResponse Mapping(Produto produto)
    {
        return new ListProductsPagedResponse(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.NumeroNotaFiscal,
            produto.Preco,
            produto.QtdDisponivel,
            produto.Categoria.Tipo);
    }
}
