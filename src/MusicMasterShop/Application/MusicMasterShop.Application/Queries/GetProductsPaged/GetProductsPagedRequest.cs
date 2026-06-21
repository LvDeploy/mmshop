using FluentValidation;
using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Queries.GetProduct;
using MusicMasterShop.Domain.Core.Pagination;

namespace MusicMasterShop.Application.Queries.GetProductsPaged;

public sealed record GetProductsPagedRequest(int PageNumber = 1, int PageSize = 10)
    : BaseRequest, IRequest<BaseResponse<PagedResult<GetProductResponse>>>
{
    public override bool IsValid()
    {
        var validator = new InlineValidator<GetProductsPagedRequest>();
        validator.RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber deve ser maior que zero");
        validator.RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize deve estar entre 1 e 100");

        ValidationResult = validator.Validate(this);
        return ValidationResult.IsValid;
    }
}
