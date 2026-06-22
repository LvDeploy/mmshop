using FluentValidation.TestHelper;
using MusicMasterShop.Application.Queries.ListProductsPaged;
using MusicMasterShop.Test.UnitTests.Builders.Request;

namespace MusicMasterShop.Test.UnitTests.Application.Queries.Handlers;

public sealed class ListProductsPagedValidatorTests
{
    private readonly ListProductsPagedValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.TestValidate(new ListProductsPagedRequestBuilder().Build())
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0, 10, nameof(ListProductsPagedRequest.PageNumber))]
    [InlineData(1, 0, nameof(ListProductsPagedRequest.PageSize))]
    [InlineData(1, 101, nameof(ListProductsPagedRequest.PageSize))]
    public void Validate_WithInvalidPagination_HasExpectedError(
        int pageNumber,
        int pageSize,
        string property)
    {
        var request = new ListProductsPagedRequestBuilder()
            .WithPageNumber(pageNumber)
            .WithPageSize(pageSize)
            .Build();

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(property);
    }
}
