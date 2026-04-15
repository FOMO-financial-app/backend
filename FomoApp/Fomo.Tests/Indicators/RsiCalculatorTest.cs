using FluentAssertions;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;
using System.Globalization;

public class RsiCalculatorTests
{
    private readonly RsiCalculator _sut = new();

    private static List<ValuesDTO> BuildValues(params decimal[] closes) =>
        closes.Select((c, i) => new ValuesDTO
        {
            Close = c.ToString(CultureInfo.InvariantCulture),
            Date = DateTime.Today.AddDays(-closes.Length + i).ToString("yyyy-MM-dd")
        }).ToList();

    [Fact]
    public void CalculateRsi_WithValidData_ReturnsCorrectValue()
    {
        var values = BuildValues(10, 20, 30, 40);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Values.Should().HaveCount(1);

        // Data is reversed internally
        // closes: [40,30,20,10]
        // diffs: -10, -10, -10
        // gain = 0, loss > 0 → RSI = 0
        result.Values[0].Should().Be(0);
    }

    [Fact]
    public void CalculateRsi_WithOnlyGains_ReturnsHundred()
    {
        var values = BuildValues(40, 30, 20, 10);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Values[0].Should().Be(100);
    }

    [Fact]
    public void CalculateRsi_WithOnlyLosses_ReturnsZero()
    {
        var values = BuildValues(10, 20, 30, 40);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Values[0].Should().Be(0);
    }

    [Fact]
    public void CalculateRsi_WithMixedData_ReturnsValueBetweenZeroAndHundred()
    {
        var values = BuildValues(10, 20, 15, 25, 20);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Values.Should().AllSatisfy(v =>
        {
            v.Should().BeGreaterThanOrEqualTo(0);
            v.Should().BeLessThanOrEqualTo(100);
        });
    }

    [Fact]
    public void CalculateRsi_WhenAllValuesEqual_ReturnsFifty()
    {
        var values = BuildValues(10, 10, 10, 10);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Values[0].Should().Be(50);
    }

    [Fact]
    public void CalculateRsi_WhenValuesNull_ReturnsEmpty()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateRsi(null, 3);
    #pragma warning restore CS8625

        result.Values.Should().BeEmpty();
        result.Date.Should().BeEmpty();
    }

    [Fact]
    public void CalculateRsi_WhenNotEnoughData_ReturnsEmpty()
    {
        var values = BuildValues(10, 20, 30);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Values.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateRsi_WhenPeriodIsLessOrEqualToZero_ReturnsEmpty(int period)
    {
        var values = BuildValues(10, 20, 30, 40);

        var result = _sut.CalculateRsi(values, period);

        result.Values.Should().BeEmpty();
    }

    [Fact]
    public void CalculateRsi_DateMatchesValuesCount()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateRsi(values, period: 3);

        result.Date.Should().HaveCount(result.Values.Count);
    }
}