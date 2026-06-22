namespace MusicMasterShop.Test.UnitTests.Controller;

public sealed class UsuarioControllerTest
{
    [Fact]
    public async Task Create_ReturnsOkAndForwardsRequest()
    {
        var mediator = new Mock<IMediator>();
        var request = new CreateUserRequestBuilder().Build();
        var response = new CreateUserResponse(Guid.NewGuid(), DateTime.UtcNow);
        mediator.Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<CreateUserResponse>().WithData(response).Build());
        var controller = new UsuarioController(
            mediator.Object,
            ControllerTestFactory.CreateCorrelationId());

        var result = await controller.Create(request, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
        mediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WhenHandlerReturnsNotFound_ReturnsNotFound()
    {
        var mediator = new Mock<IMediator>();
        Guid id = Guid.NewGuid();
        mediator.Setup(x => x.Send(
                It.Is<GetUserRequest>(request => request.Id == id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<GetUserResponse>()
                .AsFailure(ErrorType.NotFound)
                .Build());
        var controller = new UsuarioController(
            mediator.Object,
            ControllerTestFactory.CreateCorrelationId());

        var result = await controller.GetUserById(id, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}
