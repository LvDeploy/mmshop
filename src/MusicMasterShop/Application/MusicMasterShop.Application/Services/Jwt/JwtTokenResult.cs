namespace MusicMasterShop.Application.Services.Jwt;

public sealed record JwtTokenResult(string Token, DateTime ExpiresAt);
