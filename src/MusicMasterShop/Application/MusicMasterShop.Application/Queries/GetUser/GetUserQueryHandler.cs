using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.UseCases.CreateUser;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.GetUser
{
    public sealed class GetUserQueryHandler : IRequestHandler<GetUserRequest, BaseResponse<GetUserResponse>>
    {
        private readonly IUsuarioRepository _userRepository;
        public GetUserQueryHandler(IUsuarioRepository usuarioRepository)
        {
            _userRepository = usuarioRepository;
        }

        public async Task<BaseResponse<GetUserResponse>> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {;
            try
            {
                var entity = await _userRepository.Get(request.Id, cancellationToken);

                if(entity is null)
                {
                    return ResponseWrapper.Failure<GetUserResponse>(Error.Set("Registro não encontrado"), ErrorType.NotFound);
                }

                return ResponseWrapper.Success<GetUserResponse>(new GetUserResponse(entity.Id, entity.Email, entity.Nome, entity.Tipo));
            }
            catch (Exception ex)
            {
                return ResponseWrapper.Failure<GetUserResponse>(
                   Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.Message}"),
                   ErrorType.InternalError);
            }
        }
    }
}
