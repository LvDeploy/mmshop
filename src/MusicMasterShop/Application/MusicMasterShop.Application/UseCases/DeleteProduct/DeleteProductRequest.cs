using MediatR;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.UseCases.DeleteProduct;

public sealed record DeleteProductRequest(Guid Id) : IRequest<BaseResponse<bool>>;
