using MyProject.Application.Results;

namespace MyProject.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Ok_Should_Create_Success_Result()
    {
        var result = Result.Ok();

        Assert.True(result.Success);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.ErrorMessage);
    }

    [Theory]
    [InlineData("Operation failed")]
    [InlineData("")]
    [InlineData(null)]
    public void Fail_Should_Create_Failure_Result(string? errorMessage)
    {
        var result = Result.Fail(errorMessage!);

        Assert.False(result.Success);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }
}

public class GenericResultTests
{
    [Fact]
    public void Ok_Should_Create_Success_Result_With_Value()
    {
        var result = Result<int>.Ok(42);

        Assert.True(result.Success);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Ok_Should_Preserve_Null_Value_For_Reference_Type()
    {
        var result = Result<string?>.Ok(null);

        Assert.True(result.Success);
        Assert.Null(result.Value);
        Assert.Null(result.ErrorMessage);
    }

    [Theory]
    [InlineData("Detailed error")]
    [InlineData("")]
    [InlineData(null)]
    public void Fail_Should_Create_Failure_Result_Without_Value(string? errorMessage)
    {
        var result = Result<string>.Fail(errorMessage!);

        Assert.False(result.Success);
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }
}
