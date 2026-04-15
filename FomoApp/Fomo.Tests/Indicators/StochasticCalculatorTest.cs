using FluentAssertions;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;
using System.Globalization;

public class StochasticCalculatorTests
{
    private readonly StochasticCalculator _sut = new();

    private static List<ValuesDTO> BuildValues(
        decimal[] closes,
        decimal[] highs,
        decimal[] lows)
    {
        return closes.Select((c, i) => new ValuesDTO
        {
            Close = c.ToString(CultureInfo.InvariantCulture),
            High = highs[i].ToString(CultureInfo.InvariantCulture),
            Low = lows[i].ToString(CultureInfo.InvariantCulture),
            Date = DateTime.Today.AddDays(-closes.Length + i).ToString("yyyy-MM-dd")
        }).ToList();
    }

    [Fact]
    public void CalculateStochastic_WithValidData_ReturnsCorrectKValues()
    {
        var closes = new[] { 10m, 20m, 30m };
        var highs = new[] { 15m, 25m, 35m };
        var lows = new[] { 5m, 15m, 25m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 1);

        // Data is reversed internally (most recent values first)
        // min = 5, max = 35, close = 10
        // K = ((10 - 5) / (35 - 5)) * 100 = (5/30)*100 ≈ 16.66
        result.K.Should().HaveCount(1);
        result.K[0].Should().BeApproximately(16.6666m, 0.01m);
    }

    [Fact]
    public void CalculateStochastic_WhenHighEqualsLow_KIsZero()
    {
        var closes = new[] { 10m, 10m, 10m };
        var highs = new[] { 10m, 10m, 10m };
        var lows = new[] { 10m, 10m, 10m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 1);

        result.K.Should().AllSatisfy(v => v.Should().Be(0));
    }

    [Fact]
    public void CalculateStochastic_KValuesAreBetweenZeroAndHundred()
    {
        var closes = new[] { 10m, 20m, 30m, 25m, 15m };
        var highs = new[] { 15m, 25m, 35m, 30m, 20m };
        var lows = new[] { 5m, 15m, 25m, 20m, 10m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 2);

        result.K.Should().AllSatisfy(v =>
        {
            v.Should().BeGreaterThanOrEqualTo(0);
            v.Should().BeLessThanOrEqualTo(100);
        });
    }

    [Fact]
    public void CalculateStochastic_DIsSmaOfK()
    {
        var closes = new[] { 10m, 20m, 30m, 40m };
        var highs = new[] { 15m, 25m, 35m, 45m };
        var lows = new[] { 5m, 15m, 25m, 35m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 2);

        result.D.Should().HaveCount(result.K.Count - 1);

        var expectedFirstD = (result.K[0] + result.K[1]) / 2;
        result.D[0].Should().BeApproximately(expectedFirstD, 0.0001m);
    }

    [Fact]
    public void CalculateStochastic_WhenValuesNull_ReturnsEmpty()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateStochastic(null, 3, 2);
    #pragma warning restore CS8625

        result.K.Should().BeEmpty();
        result.D.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateStochastic_WhenPeriodIsLessThanOne_ReturnsEmpty(int period)
    {
        var closes = new[] { 10m, 20m, 30m, 40m };
        var highs = new[] { 15m, 25m, 35m, 45m };
        var lows = new[] { 5m, 15m, 25m, 35m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period, smaperiod: 2);

        result.K.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateStochastic_WhenSmaPeriodIsLessThanOne_ReturnsEmpty(int smaperiod)
    {
        var closes = new[] { 10m, 20m, 30m, 40m };
        var highs = new[] { 15m, 25m, 35m, 45m };
        var lows = new[] { 5m, 15m, 25m, 35m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod);

        result.D.Should().BeEmpty();
    }

    [Fact]
    public void CalculateStochastic_WhenNotEnoughData_ReturnsEmpty()
    {
        var closes = new[] { 10m, 20m };
        var highs = new[] { 15m, 25m };
        var lows = new[] { 5m, 15m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 2);

        result.K.Should().BeEmpty();
    }

    [Fact]
    public void CalculateStochastic_WhenSmaPeriodExceedsAvailableKValues_ReturnsEmpty()
    {
        var closes = new[] { 10m, 20m, 30m };
        var highs = new[] { 15m, 25m, 35m };
        var lows = new[] { 5m, 15m, 25m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 5);

        result.D.Should().BeEmpty();
    }

    [Fact]
    public void CalculateStochastic_DateListMatchesValuesCount()
    {
        var closes = new[] { 10m, 20m, 30m, 40m };
        var highs = new[] { 15m, 25m, 35m, 45m };
        var lows = new[] { 5m, 15m, 25m, 35m };

        var values = BuildValues(closes, highs, lows);

        var result = _sut.CalculateStochastic(values, period: 3, smaperiod: 2);

        result.Kdate.Should().HaveCount(result.K.Count);
        result.Ddate.Should().HaveCount(result.D.Count);
    }
}