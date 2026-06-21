using FluentValidation;
using MusicMasterShop.Application.UseCases.CreateProduct;

namespace MusicMasterShop.Application.UseCases.UpdateProduct;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("Id do produto não pode ser vazio");

        RuleFor(request => request.Nome)
            .NotEmpty()
            .WithMessage("Campo Nome não pode ser vazio");
        RuleFor(request => request.Descricao)
            .NotEmpty()
            .WithMessage("Campo Descricao não pode ser vazio");
        RuleFor(request => request.Modelo)
            .NotEmpty()
            .WithMessage("Campo Modelo não pode ser vazio");
        RuleFor(request => request.Marca)
            .NotEmpty()
            .WithMessage("Campo Marca não pode ser vazio");
        RuleFor(request => request.SerialNumber)
            .NotEmpty()
            .WithMessage("Campo SerialNumber não pode ser vazio");
        RuleFor(request => request.GarantiaEmDias)
            .GreaterThan(0)
            .WithMessage("Campo GarantiaEmDias deve ser maior que 0 dias");
        RuleFor(request => request.Preco)
            .GreaterThan(0)
            .WithMessage("Campo Preco deve ser maior que 0.0");
        RuleFor(request => request.Categoria)
            .NotNull()
            .IsInEnum()
            .WithMessage("Campo Categoria é inválido");
        RuleFor(request => request.Dimensoes)
            .NotNull()
            .WithMessage("Campo Dimensoes não pode ser vazio")
            .SetValidator(new CreateProductDimensaoValidator());
    }
}
