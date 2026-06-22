using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateProduct
{
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductRequest, BaseResponse<CreateProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        public CreateProductCommandHandler(
            IUnitOfWork unitOfWork,
            ICategoriaRepository categoriaRepository,
            IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
            _unitOfWork = unitOfWork;
            _categoriaRepository = categoriaRepository;
        }
        public async Task<BaseResponse<CreateProductResponse>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                return ResponseWrapper.Failure<CreateProductResponse>(request.ValidationResult.Errors, ErrorType.BadRequest);
            }

             var categoriaEntity = await _categoriaRepository.GetByTipoAsync(request.TipoCategoriaId!.Value, cancellationToken);
            if(categoriaEntity == null)
            {
                return ResponseWrapper.Failure<CreateProductResponse>(
                     Error.Set("Categoria não encontrada"),
                     ErrorType.Unauthorized);
            }

            var produtoEntity = Produto.Create(nome: request.Nome, 
                descricao: request.Descricao, 
                preco: request.Preco,
                categoria: categoriaEntity);

            _produtoRepository.Create(produtoEntity);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResponseWrapper.Success<CreateProductResponse>(new CreateProductResponse(produtoEntity.Id, produtoEntity.CreatedAt));
        }
    }
}
