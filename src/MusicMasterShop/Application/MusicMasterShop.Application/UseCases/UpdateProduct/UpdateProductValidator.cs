using FluentValidation;
using MusicMasterShop.Application.UseCases.CreateProduct;

namespace MusicMasterShop.Application.UseCases.UpdateProduct;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(x => $"Campo {x.Id} não pode ser vazio");
        RuleFor(x => x.Nome)
            .MinimumLength(1)
            .WithMessage(x => $"Campo {x.Nome} não pode ser vazio")
            .When(x => x.Nome != null, ApplyConditionTo.CurrentValidator);
        RuleFor(x => x.Descricao)
            .MinimumLength(1)
            .WithMessage(x => $"Campo {x.Descricao}  não pode ser vazio")
            .When(x => x.Descricao != null, ApplyConditionTo.CurrentValidator);
        RuleFor(x => x.Preco)
            .GreaterThan(0)
            .WithMessage(x => $"Campo {x.Preco}  deve ser maior que 0.0")
            .When(x => x.Preco > 0, ApplyConditionTo.CurrentValidator);
        RuleFor(x => x.TipoCategoriaId)
            .IsInEnum()
            .WithMessage(x => $"Campo {x.TipoCategoriaId} é inválido")
            .When(x => x.TipoCategoriaId != null, ApplyConditionTo.CurrentValidator);
    }
}
