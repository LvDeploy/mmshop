using MediatR;
using Microsoft.AspNetCore.Identity;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Services.Jwt;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginRequest, BaseResponse<LoginResponse>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<BaseResponse<LoginResponse>> Handle(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return ResponseWrapper.Failure<LoginResponse>(
                    request.ValidationResult.Errors,
                    ErrorType.BadRequest);
            }

            Usuario? usuario = await _usuarioRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (usuario is null || !usuario.Ativo || !PasswordIsValid(usuario, request.Senha))
            {
                return ResponseWrapper.Failure<LoginResponse>(
                    Error.Set("E-mail ou senha inválidos"),
                    ErrorType.Unauthorized);
            }

            JwtTokenResult token = _jwtTokenService.GenerateToken(usuario);

            return ResponseWrapper.Success(new LoginResponse(token.Token, token.ExpiresAt));
        }
        catch (Exception ex)
        {
            return ResponseWrapper.Failure<LoginResponse>(
               Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.Message}"),
               ErrorType.InternalError);
        }
    }

    private bool PasswordIsValid(Usuario usuario, string password)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.Senha,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}
