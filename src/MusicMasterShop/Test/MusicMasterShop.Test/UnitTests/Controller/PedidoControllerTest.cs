namespace MusicMasterShop.Test.UnitTests.Controller;

public sealed class PedidoControllerTest
{
    [Fact]
    public async Task CreateCart_WhenUserIsNotSeller_ReturnsForbidWithoutMediator()
    {
        var mediator = new Mock<IMediator>();
        var userInfo = new Mock<IUserInfo>();
        userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Administrador);
        var controller = CreateController(mediator, userInfo);

        var result = await controller.CreateCart(
            new CreateCartRequestBuilder().Build(),
            CancellationToken.None);

        Assert.IsType<ForbidResult>(result.Result);
        mediator.Verify(
            x => x.Send(It.IsAny<IRequest<CreateCartResponse>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetOrder_WhenSeller_UsesRouteIdAndReturnsOk()
    {
        var mediator = new Mock<IMediator>();
        var userInfo = new Mock<IUserInfo>();
        userInfo.SetupGet(x => x.TipoUsuario).Returns(TipoUsuario.Vendedor);
        Guid pedidoId = Guid.NewGuid();
        var response = new GetOrderResponse(
            pedidoId,
            Guid.NewGuid(),
            "12345678901",
            StatusPedido.Validado,
            DateTime.UtcNow,
            null,
            []);
        mediator.Setup(x => x.Send(
                It.Is<GetOrderRequest>(request => request.PedidoId == pedidoId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<GetOrderResponse>().WithData(response).Build());
        var controller = CreateController(mediator, userInfo);

        var result = await controller.GetOrder(pedidoId, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    private static PedidoController CreateController(
        Mock<IMediator> mediator,
        Mock<IUserInfo> userInfo) =>
        new(mediator.Object, ControllerTestFactory.CreateCorrelationId(), userInfo.Object);
}
