using MediatR;
using MusicMasterShop.Application.Abstractions.Request;
using MusicMasterShop.Application.Abstractions.Response;

namespace MusicMasterShop.Application.UseCases.Login;

public sealed record LoginRequest(string Email, string Senha)
    : BaseRequest, IRequest<BaseResponse<LoginResponse>>
{
    public override bool IsValid()
    {
        ValidationResult = new LoginValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
