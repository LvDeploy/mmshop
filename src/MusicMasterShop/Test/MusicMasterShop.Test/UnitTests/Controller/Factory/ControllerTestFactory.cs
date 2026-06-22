namespace MusicMasterShop.Test.UnitTests.Controller.Factory;

internal static class ControllerTestFactory
{
    public static CorrelationId CreateCorrelationId()
    {
        var context = new DefaultHttpContext();
        context.Items["X-Correlation-Id"] = "controller-test";
        return new CorrelationId(new HttpContextAccessor { HttpContext = context });
    }
}
