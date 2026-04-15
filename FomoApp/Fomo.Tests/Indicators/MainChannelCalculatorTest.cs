using FluentAssertions;
using Fomo.Application.DTO.StockDataDTO;
using Fomo.Application.Services.Indicators;
using System.Globalization;

public class MainChannelCalculatorTests
{
    private readonly MainChannelCalculator _sut = new();

    private static List<ValuesDTO> BuildValues(params (decimal high, decimal low, decimal close)[] values) =>
        values.Select((v, i) => new ValuesDTO
        {
            High = v.high.ToString(CultureInfo.InvariantCulture),
            Low = v.low.ToString(CultureInfo.InvariantCulture),
            Close = v.close.ToString(CultureInfo.InvariantCulture),
            Date = DateTime.Today.AddDays(-values.Length + i).ToString("yyyy-MM-dd")
        }).ToList();

    [Fact]
    public void CalculateMainChannel_WithValidData_ReturnsValues()
    {
        var values = BuildValues(
            (10, 5, 8),
            (20, 10, 15),
            (30, 20, 25),
            (40, 30, 35)
        );

        var result = _sut.CalculateMainChannel(values);

        result.Regression.Should().NotBeEmpty();
        result.Upper.Should().NotBeEmpty();
        result.Lower.Should().NotBeEmpty();
    }

    [Fact]
    public void CalculateMainChannel_AllListsHaveSameCount()
    {
        var values = BuildValues(
            (10, 5, 8),
            (20, 10, 15),
            (30, 20, 25)
        );

        var result = _sut.CalculateMainChannel(values);

        result.Regression.Should().HaveCount(values.Count);
        result.Upper.Should().HaveCount(values.Count);
        result.Lower.Should().HaveCount(values.Count);
    }

    [Fact]
    public void CalculateMainChannel_UpperIsAboveRegression_AndLowerIsBelow()
    {
        var values = BuildValues(
            (10, 5, 8),
            (20, 10, 15),
            (30, 20, 25),
            (40, 30, 35)
        );

        var result = _sut.CalculateMainChannel(values);

        for (int i = 0; i < result.Regression.Count; i++)
        {
            result.Upper[i].Should().BeGreaterThanOrEqualTo(result.Regression[i]);
            result.Lower[i].Should().BeLessThanOrEqualTo(result.Regression[i]);
        }
    }

    [Fact]
    public void CalculateMainChannel_WithConstantValues_ProducesFlatChannel()
    {
        var values = BuildValues(
            (10, 10, 10),
            (10, 10, 10),
            (10, 10, 10),
            (10, 10, 10)
        );

        var result = _sut.CalculateMainChannel(values);

        result.Regression.Should().AllSatisfy(v => v.Should().BeApproximately(10, 0.0001m));
        result.Upper.Should().AllSatisfy(v => v.Should().BeApproximately(10, 0.0001m));
        result.Lower.Should().AllSatisfy(v => v.Should().BeApproximately(10, 0.0001m));
    }

    [Fact]
    public void CalculateMainChannel_ChannelContainsAllPrices()
    {
        var values = BuildValues(
            (10, 5, 8),
            (20, 10, 15),
            (30, 20, 25),
            (40, 30, 35)
        );

        var reversedValues = values
            .Select(v => new
            {
                High = decimal.Parse(v.High, CultureInfo.InvariantCulture),
                Low = decimal.Parse(v.Low, CultureInfo.InvariantCulture)
            })
            .Reverse()
            .ToList();

        var result = _sut.CalculateMainChannel(values);


        for (int i = 0; i < values.Count; i++)
        {
            result.Upper[i].Should().BeGreaterThanOrEqualTo(reversedValues[i].High);
            result.Lower[i].Should().BeLessThanOrEqualTo(reversedValues[i].Low);
        }
    }

    [Fact]
    public void CalculateMainChannel_WhenValuesNull_ReturnsEmpty()
    {
    #pragma warning disable CS8625
        var result = _sut.CalculateMainChannel(null);
    #pragma warning restore CS8625

        result.Regression.Should().BeEmpty();
        result.Upper.Should().BeEmpty();
        result.Lower.Should().BeEmpty();
    }

    [Fact]
    public void CalculateMainChannel_WithSingleValue_ReturnsFlatChannel()
    {
        var values = BuildValues((10, 5, 8));

        var result = _sut.CalculateMainChannel(values);

        result.Regression.Should().HaveCount(1);
    }

    [Fact]
    public void CalculateMainChannel_WhenValuesEmpty_ReturnsEmpty()
    {
        var result = _sut.CalculateMainChannel(new List<ValuesDTO>());

        result.Regression.Should().BeEmpty();
        result.Upper.Should().BeEmpty();
        result.Lower.Should().BeEmpty();
    }
}