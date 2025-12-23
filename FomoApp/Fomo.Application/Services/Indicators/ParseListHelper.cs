using Fomo.Application.DTO.StockDataDTO;
using System.Globalization;

namespace Fomo.Application.Services.Indicators
{
    public class ParseListHelper
    {
        public List<decimal> ParseList (List<ValuesDTO> values, Func<ValuesDTO, string> property)
        {
            if (Convert.ToString(property) == "datetime")
            {
                return new List<decimal>();
            }

            var valuesStr = values
                .Select(property)
                .ToList();

            var valuesd = new List<decimal>();

            foreach (string value in valuesStr)
            {
                decimal Price = 0;
                decimal.TryParse(value, CultureInfo.InvariantCulture, out Price);
                valuesd.Add(Price);
            }

            valuesd.Reverse();

            return valuesd;
        }

        public List<string> GetDate(List<ValuesDTO> values)
        {
            var datelist = values
                .Select(v => v.Date)
                .ToList();

            datelist.Reverse();

            return datelist;
        }
    }
}
