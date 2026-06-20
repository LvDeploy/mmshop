using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateUser
{
    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserRequest, BaseResponse<CreateUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioRepository _userRepository;
        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUsuarioRepository usuarioRepository)
        {
            _userRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<CreateUserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                return ResponseWrapper.Failure<CreateUserResponse>(request.ValidationResult.Errors, ErrorType.BadRequest);
            }

            var entity = Usuario.Create(email: request.Email,
                nome: request.Nome,
                senha: request.Senha,
                tipo: request.TipoUsuario!.Value);

            _userRepository.Create(entity);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResponseWrapper.Success<CreateUserResponse>(new CreateUserResponse(entity.Id, entity.CreatedAt));
        }
    }
}
