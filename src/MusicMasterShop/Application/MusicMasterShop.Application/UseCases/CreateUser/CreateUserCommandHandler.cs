using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateUser
{
    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserRequest, BaseResponse<CreateUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioRepository _userRepository;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<Usuario> _passwordHasher;

        public CreateUserCommandHandler(
            IUnitOfWork unitOfWork,
            IUsuarioRepository usuarioRepository,
            Microsoft.AspNetCore.Identity.IPasswordHasher<Usuario> passwordHasher)
        {
            _userRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<BaseResponse<CreateUserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.IsValid())
                {
                    return ResponseWrapper.Failure<CreateUserResponse>(request.ValidationResult.Errors, ErrorType.BadRequest);
                }

                var entity = Usuario.Create(email: request.Email.Trim(),
                    nome: request.Nome,
                    senha: string.Empty,
                    tipo: request.TipoUsuario!.Value);

                string passwordHash = _passwordHasher.HashPassword(entity, request.Senha);
                entity.SetPassword(passwordHash);

                _userRepository.Create(entity);
                await _unitOfWork.CommitAsync(cancellationToken);

                return ResponseWrapper.Success<CreateUserResponse>(new CreateUserResponse(entity.Id, entity.CreatedAt));
            }
            catch (Exception ex)
            {
                return ResponseWrapper.Failure<CreateUserResponse>(
                   Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.Message}"),
                   ErrorType.InternalError);
            }
        }
    }
}
