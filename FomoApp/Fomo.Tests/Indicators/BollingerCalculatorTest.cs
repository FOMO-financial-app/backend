using FluentAssertions;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;
using System.Globalization;

public class BollingerCalculatorTests
{
    private readonly BollingerCalculator _sut = new();

    private static List<ValuesDTO> BuildValues(params decimal[] closes) =>
        closes.Select((c, i) => new ValuesDTO
        {
            Close = c.ToString(CultureInfo.InvariantCulture),
            Date = DateTime.Today.AddDays(-closes.Length + i).ToString("yyyy-MM-dd")
        }).ToList();

    [Fact]
    public void CalculateBollinger_WithValidData_ReturnsCorrectBands()
    {
        var values = BuildValues(10, 20, 30);
        var expectedSma = 20m;

        // σ = sqrt(((10-20)^2 + (20-20)^2 + (30-20)^2)/3)
        // = sqrt((100 + 0 + 100)/3) = sqrt(66.666...) ≈ 8.1649
        var expectedStdDev = (decimal)Math.Sqrt(200.0 / 3.0);

        var result = _sut.CalculateBollinger(values, period: 3, k: 1);

        result.UpperBand[0].Should().BeApproximately(expectedSma + expectedStdDev, 0.0001m);
        result.LowerBand[0].Should().BeApproximately(expectedSma - expectedStdDev, 0.0001m);
    }

    [Fact]
    public void CalculateBollinger_WithConstantValues_BandsAreEqual()
    {
        var values = BuildValues(10, 10, 10, 10);

        var result = _sut.CalculateBollinger(values, period: 3, k: 2);

        result.UpperBand.Should().AllSatisfy(v => v.Should().Be(10));
        result.LowerBand.Should().AllSatisfy(v => v.Should().Be(10));
    }

    [Fact]
    public void CalculateBollinger_BandsAreSymmetricAroundSma()
    {
        var values = BuildValues(10, 20, 30);
        var expectedSma = 20m;

        var result = _sut.CalculateBollinger(values, period: 3, k: 2);

        var upperDiff = result.UpperBand[0] - expectedSma;
        var lowerDiff = expectedSma - result.LowerBand[0];

        upperDiff.Should().BeApproximately(lowerDiff, 0.0001m);
    }

    [Fact]
    public void CalculateBollinger_WithValidData_UpperBandIsAboveLowerBand()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateBollinger(values, period: 3, k: 2);

        result.Should().NotBeNull();
        result.UpperBand.Should().HaveCount(3);

        for (int i = 0; i < result.UpperBand.Count; i++)
            result.UpperBand[i].Should().BeGreaterThan(result.LowerBand[i]);
    }

    [Fact]
    public void CalculateBollinger_WhenValuesNull_ReturnsEmptyBands()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateBollinger(null, period: 3, k: 2);
    #pragma warning restore CS8625

        result.UpperBand.Should().BeEmpty();
        result.LowerBand.Should().BeEmpty();
        result.Date.Should().BeEmpty();
    }

    [Fact]
    public void CalculateBollinger_WhenKLessThanOne_ReturnsEmptyBands()
    {
        var values = BuildValues(10, 20, 30);

        var result = _sut.CalculateBollinger(values, period: 3, k: 0);

        result.UpperBand.Should().BeEmpty();
    }

    [Fact]
    public void CalculateBollinger_WhenPeriodLessThanOne_ReturnsEmptyBands()
    {
        var values = BuildValues(10, 20, 30);

        var result = _sut.CalculateBollinger(values, period: -1, k: 2);

        result.UpperBand.Should().BeEmpty();
    }

    [Fact]
    public void CalculateBollinger_DateListMatchesBandCount()
    {
        var values = BuildValues(10, 20, 30, 40, 50);

        var result = _sut.CalculateBollinger(values, period: 3, k: 2);

        result.Date.Should().HaveCount(result.UpperBand.Count);
        result.Date.Should().HaveCount(result.LowerBand.Count);
    }
}