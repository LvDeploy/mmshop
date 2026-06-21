using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

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

            var dimensaoVo = new Dimensao(request.Dimensoes.AlturaCm, request.Dimensoes.LarguraCm, request.Dimensoes.ComprimentoCm, request.Dimensoes.PesoKg);
            var categoriaEntity = await _categoriaRepository.GetByTipoAsync(request.Categoria!.Value, cancellationToken);
            if(categoriaEntity == null)
            {
                return ResponseWrapper.Failure<CreateProductResponse>(
                     Error.Set("Categoria não encontrada"),
                     ErrorType.Unauthorized);
            }

            var produtoEntity = Produto.Create(nome: request.Nome, 
                descricao: request.Descricao, 
                modelo: request.Modelo, 
                marca: request.Marca, 
                serialNumber: request.SerialNumber, 
                garantiaEmDias: request.GarantiaEmDias,
                preco: request.Preco,
                dimensao: dimensaoVo,
                categoria: categoriaEntity);

            _produtoRepository.Create(produtoEntity);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResponseWrapper.Success<CreateProductResponse>(new CreateProductResponse(produtoEntity.Id, produtoEntity.CreatedAt));
        }
    }
}
