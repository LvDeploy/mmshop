using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.WebApi.Controllers.Base;

namespace MusicMasterShop.Test.UnitTests.Controller;

public sealed class ApiControllerBaseTests
{
    private const string CorrelationIdValue = "correlation-id";
    private readonly ApiControllerBase _controller;

    public ApiControllerBaseTests()
    {
        var context = new DefaultHttpContext();
        context.Items["X-Correlation-Id"] = CorrelationIdValue;
        var correlationId = new CorrelationId(
            new HttpContextAccessor { HttpContext = context });
        _controller = new ApiControllerBase(correlationId);
    }

    [Fact]
    public void CreateResponse_WithSuccess_ReturnsOk()
    {
        var response = new ResponseBuilder<string>()
            .WithData("payload")
            .WithMessage("created")
            .Build();

        ActionResult<string> action = _controller.CreateResponse(response);

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var body = Assert.IsType<SuccessResult<string>>(ok.Value);
        Assert.Equal(CorrelationIdValue, body.CorrelationId);
        Assert.Equal("payload", body.Data);
        Assert.Equal("created", body.Message);
    }

    [Theory]
    [InlineData(ErrorType.BadRequest, typeof(BadRequestObjectResult), 400)]
    [InlineData(ErrorType.NotFound, typeof(NotFoundObjectResult), 404)]
    public void CreateResponse_WithFailureContainingBody_ReturnsExpectedStatus(
        ErrorType errorType,
        Type resultType,
        int statusCode)
    {
        var response = new ResponseBuilder<string>()
            .AsFailure(errorType, "failure")
            .Build();

        ActionResult<string> action = _controller.CreateResponse(response);

        Assert.IsType(resultType, action.Result);
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(action.Result);
        Assert.Equal(statusCode, objectResult.StatusCode);
        var body = Assert.IsType<FailureResult>(objectResult.Value);
        Assert.Equal(CorrelationIdValue, body.CorrelationId);
        Assert.Equal("failure", Assert.Single(body.Errors).Detail);
    }

    [Fact]
    public void CreateResponse_WithUnauthorized_ReturnsUnauthorized()
    {
        var response = new ResponseBuilder<string>()
            .AsFailure(ErrorType.Unauthorized)
            .Build();

        ActionResult<string> action = _controller.CreateResponse(response);

        Assert.IsType<UnauthorizedResult>(action.Result);
    }

    [Fact]
    public void CreateResponse_WithForbidden_ReturnsForbid()
    {
        var response = new ResponseBuilder<string>()
            .AsFailure(ErrorType.Forbidden)
            .Build();

        ActionResult<string> action = _controller.CreateResponse(response);

        Assert.IsType<ForbidResult>(action.Result);
    }

    [Fact]
    public void CreateResponse_WithInternalError_ReturnsInternalServerError()
    {
        var response = new ResponseBuilder<string>()
            .AsFailure(ErrorType.InternalError)
            .Build();

        ActionResult<string> action = _controller.CreateResponse(response);

        var result = Assert.IsType<StatusCodeResult>(action.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
    }
}
