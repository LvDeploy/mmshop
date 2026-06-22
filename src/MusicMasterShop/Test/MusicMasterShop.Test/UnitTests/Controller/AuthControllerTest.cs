namespace MusicMasterShop.Test.UnitTests.Controller;

public sealed class AuthControllerTest
{
    [Fact]
    public async Task Login_ReturnsStatusProducedByHandler()
    {
        var mediator = new Mock<IMediator>();
        var request = new LoginRequestBuilder().Build();
        mediator.Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<LoginResponse>()
                .AsFailure(ErrorType.Unauthorized)
                .Build());
        var controller = new AuthController(
            mediator.Object,
            ControllerTestFactory.CreateCorrelationId());

        var result = await controller.Login(request, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}
