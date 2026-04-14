using Fomo.Application.Services.Indicators;
using FluentAssertions;

namespace Fomo.Tests.Indicators;

public class SmaCalculatorTests
{
    private readonly SmaCalculator _sut = new();

    [Fact]
    public void CalculateSMA_WithValidData_ReturnsCorrectValues()
    {
        var values = new List<decimal> { 10, 20, 30 };

        var result = _sut.CalculateSMA(values, period: 3);

        result.Should().HaveCount(1);
        result[0].Should().Be(20m);
    }

    [Fact]
    public void CalculateSMA_WithMoreDataThanPeriod_ReturnsMultipleValues()
    {
        var values = new List<decimal> { 10, 20, 30, 40, 50 };

        var result = _sut.CalculateSMA(values, period: 3);

        result.Should().HaveCount(3);
        result[0].Should().Be(20m);
        result[1].Should().Be(30m);
        result[2].Should().Be(40m);
    }

    [Fact]
    public void CalculateSMA_WhenValuesLessThanPeriod_ReturnsEmptyList()
    {
        var values = new List<decimal> { 10, 20 };

        var result = _sut.CalculateSMA(values, period: 5);

        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateSMA_WhenPeriodIsZero_ReturnsEmptyList()
    {
        var values = new List<decimal> { 10, 20, 30 };

        var result = _sut.CalculateSMA(values, period: 0);

        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateSMA_WhenValuesIsNull_ReturnsEmptyList()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateSMA(null, period: 3);
    #pragma warning restore CS8625
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateSMA_WithPeriodOne_ReturnsSameValues()
    {
        var values = new List<decimal> { 5, 10, 15 };

        var result = _sut.CalculateSMA(values, period: 1);

        result.Should().Equal(5m, 10m, 15m);
    }
}