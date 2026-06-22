using FluentValidation;

namespace MusicMasterShop.Application.Queries.ListProductsPaged
{
    public class ListProductsPagedValidator
    : AbstractValidator<ListProductsPagedRequest>
    {
        public ListProductsPagedValidator()
        {
            RuleFor(x => x.PageNumber)
              .GreaterThan(0)
              .WithMessage(x => $"Campo {x.PageNumber} deve ser maior que zero");
            RuleFor(x => x.PageSize)
              .InclusiveBetween(1, 100)
              .WithMessage(x => $"Campo {x.PageSize} deve estar entre 1 e 100");
        }
    }
}
