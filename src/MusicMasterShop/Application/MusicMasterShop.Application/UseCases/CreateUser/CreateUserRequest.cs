using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.CreateUser
{
    public sealed record CreateUserRequest(string Email, string Nome, TipoUsuario? TipoUsuario, string Senha) : BaseRequest, IRequest<BaseResponse<CreateUserResponse>>
    {
        public override bool IsValid()
        {
            ValidationResult = new CreateUserValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
