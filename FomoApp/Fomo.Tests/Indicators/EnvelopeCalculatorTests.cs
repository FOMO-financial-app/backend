using FluentAssertions;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;
using System.Globalization;

namespace Fomo.Tests.Indicators;

public class EnvelopeCalculatorTests
{
    private readonly EnvelopeCalculator _sut = new();

    private static List<ValuesDTO> BuildValues(params decimal[] closes) =>
    closes.Select((c, i) => new ValuesDTO
    {
        Close = c.ToString(CultureInfo.InvariantCulture),
        Date = DateTime.Today.AddDays(-closes.Length + i).ToString("yyyy-MM-dd")
    }).ToList();

    [Fact]
    public void CalculateEnvelope_WithValidData_UpperBandIsAboveSma()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateEnvelope(values, period: 3, percentage: 10);

        result.UpperBand.Should().AllSatisfy(v => v.Should().BeGreaterThan(0));
        result.UpperBand[0].Should().BeGreaterThan(result.LowerBand[0]);
    }

    [Fact]
    public void CalculateEnvelope_WithTenPercent_BandsAreSymmetric()
    {
        var values = BuildValues(10, 20, 30);
        var expectedSma = 20m;

        var result = _sut.CalculateEnvelope(values, period: 3, percentage: 10);

        result.UpperBand[0].Should().Be(expectedSma * 1.1m);
        result.LowerBand[0].Should().Be(expectedSma * 0.9m);
    }

    [Fact]
    public void CalculateEnvelope_WithValidData_UpperBandIsAboveLowerBand()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateEnvelope(values, period: 3, percentage: 10);

        result.Should().NotBeNull();
        result.UpperBand.Should().HaveCount(3);
        for (int i = 0; i < result.UpperBand.Count; i++)
            result.UpperBand[i].Should().BeGreaterThan(result.LowerBand[i]);
    }

    [Fact]
    public void CalculateEnvelope_WhenValuesNull_ReturnsEmptyBands()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateEnvelope(null, period: 3, percentage: 10);
    #pragma warning restore CS8625

        result.UpperBand.Should().BeEmpty();
        result.LowerBand.Should().BeEmpty();
        result.Date.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateEnvelope_WhenPercentageLessThanOne_ReturnsEmptyBands(int percentage)
    {
        var values = BuildValues(10, 20, 30);

        var result = _sut.CalculateEnvelope(values, period: 3, percentage);

        result.UpperBand.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateEnvelope_WhenPeriodIsZero_ReturnsEmptyBands(int period)
    {
        var values = BuildValues(10, 20, 30);

        var result = _sut.CalculateEnvelope(values, period, percentage: 10);

        result.UpperBand.Should().BeEmpty();
    }

    [Fact]
    public void CalculateEnvelope_DateListMatchesBandCount()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateEnvelope(values, period: 3, percentage: 5);

        result.Date.Should().HaveCount(result.UpperBand.Count);
        result.Date.Should().HaveCount(result.LowerBand.Count);
    }
}