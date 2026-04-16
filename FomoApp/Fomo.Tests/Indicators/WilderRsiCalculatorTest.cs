using FluentAssertions;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;
using System.Globalization;

public class WilderRsiCalculatorTests
{
    private readonly WilderRsiCalculator _sut = new();

    private static List<ValuesDTO> BuildValues(params decimal[] closes) =>
        closes.Select((c, i) => new ValuesDTO
        {
            Close = c.ToString(CultureInfo.InvariantCulture),
            Date = DateTime.Today.AddDays(-closes.Length + i).ToString("yyyy-MM-dd")
        }).ToList();

    [Fact]
    public void CalculateWilderRsi_WithValidData_ReturnsValues()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Values.Should().NotBeEmpty();
        result.Values.Should().OnlyContain(v => v >= 0 && v <= 100);
    }

    [Fact]
    public void CalculateWilderRsi_WithOnlyGains_ReturnsHundred()
    {
        var values = BuildValues(50, 40, 30, 20, 10);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Values.Should().AllSatisfy(v => v.Should().Be(100));
    }

    [Fact]
    public void CalculateWilderRsi_WithOnlyLosses_ReturnsZero()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Values.Should().AllSatisfy(v => v.Should().Be(0));
    }

    [Fact]
    public void CalculateWilderRsi_WithMixedData_ReturnsValueBetweenZeroAndHundred()
    {
        var values = BuildValues(10, 20, 15, 25, 20, 30);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Values.Should().AllSatisfy(v =>
        {
            v.Should().BeGreaterThanOrEqualTo(0);
            v.Should().BeLessThanOrEqualTo(100);
        });
    }

    [Fact]
    public void CalculateWilderRsi_WhenAllValuesEqual_ReturnsFifty()
    {
        var values = BuildValues(10, 10, 10, 10, 10);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Values.Should().AllSatisfy(v => v.Should().Be(50));
    }

    [Fact]
    public void CalculateWilderRsi_WhenValuesNull_ReturnsEmpty()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateWilderRsi(null, 3);
    #pragma warning restore CS8625

        result.Values.Should().BeEmpty();
        result.Date.Should().BeEmpty();
    }

    [Fact]
    public void CalculateWilderRsi_WhenNotEnoughData_ReturnsEmpty()
    {
        var values = BuildValues(10, 20, 30);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Values.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateWilderRsi_WhenPeriodIsLessOrEqualToZero_ReturnsEmpty(int period)
    {
        var values = BuildValues(10, 20, 30, 40);

        var result = _sut.CalculateWilderRsi(values, period);

        result.Values.Should().BeEmpty();
    }

    [Fact]
    public void CalculateWilderRsi_DateMatchesValuesCount()
    {
        var values = BuildValues(10, 20, 30, 40, 50, 60);

        var result = _sut.CalculateWilderRsi(values, period: 3);

        result.Date.Should().HaveCount(result.Values.Count);
    }
}