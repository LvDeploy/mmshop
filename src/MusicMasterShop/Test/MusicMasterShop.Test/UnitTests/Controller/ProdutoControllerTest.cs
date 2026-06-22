namespace MusicMasterShop.Test.UnitTests.Controller;

public sealed class ProdutoControllerTest
{
    [Fact]
    public async Task Create_WhenUserIsNotAdministrator_ReturnsForbidWithoutMediator()
    {
        var mediator = new Mock<IMediator>();
        var userInfo = new Mock<IUserInfo>();
        userInfo.SetupGet(x => x.IsAdministrator).Returns(false);
        var controller = CreateController(mediator, userInfo);

        var result = await controller.Create(
            new CreateProductRequestBuilder().Build(),
            CancellationToken.None);

        Assert.IsType<ForbidResult>(result.Result);
        mediator.Verify(
            x => x.Send(It.IsAny<IRequest<CreateProductResponse>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ListProductsPaged_WhenAdministrator_ForwardsPaginationAndReturnsOk()
    {
        var mediator = new Mock<IMediator>();
        var userInfo = Administrator();
        var page = new PagedResult<ListProductsPagedResponse>
        {
            Items = [],
            CurrentPage = 3,
            PageSize = 25,
            TotalCount = 0
        };
        mediator.Setup(x => x.Send(
                It.Is<ListProductsPagedRequest>(
                    request => request.PageNumber == 3 && request.PageSize == 25),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<PagedResult<ListProductsPagedResponse>>()
                .WithData(page)
                .Build());
        var controller = CreateController(mediator, userInfo);

        var result = await controller.ListProductsPaged(3, 25, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProductById_WhenHandlerReturnsNotFound_ReturnsNotFound()
    {
        var mediator = new Mock<IMediator>();
        Guid id = Guid.NewGuid();
        mediator.Setup(x => x.Send(
                It.Is<GetProductRequest>(request => request.Id == id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<GetProductResponse>()
                .AsFailure(ErrorType.NotFound)
                .Build());
        var controller = CreateController(mediator, Administrator());

        var result = await controller.GetProductById(id, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenAdministrator_UsesRouteId()
    {
        var mediator = new Mock<IMediator>();
        Guid id = Guid.NewGuid();
        var request = new UpdateProductRequestBuilder().Build();
        var response = new UpdateProductResponse(id, DateTime.UtcNow);
        mediator.Setup(x => x.Send(
                It.Is<UpdateProductRequest>(value => value.Id == id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResponseBuilder<UpdateProductResponse>().WithData(response).Build());
        var controller = CreateController(mediator, Administrator());

        var result = await controller.Update(id, request, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    private static Mock<IUserInfo> Administrator()
    {
        var userInfo = new Mock<IUserInfo>();
        userInfo.SetupGet(x => x.IsAdministrator).Returns(true);
        return userInfo;
    }

    private static ProdutoController CreateController(
        Mock<IMediator> mediator,
        Mock<IUserInfo> userInfo) =>
        new(mediator.Object, ControllerTestFactory.CreateCorrelationId(), userInfo.Object);
}
