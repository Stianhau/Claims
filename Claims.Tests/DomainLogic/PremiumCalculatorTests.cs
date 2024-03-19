using Xunit;

namespace Claims.Tests
{
    public class PremiumCalculatorTests
    {
        [Theory]
        [InlineData("2024-01-01", "2024-01-01", CoverType.BulkCarrier, 1625)] // Only one day, no discounts default
        [InlineData("2024-01-01", "2024-01-01", CoverType.Yacht, 1375)] // Only one day, no discounts yacht
        [InlineData("2024-01-01", "2024-01-31", CoverType.BulkCarrier, 50342.5)] // Exactly 31 days, one day discount default
        [InlineData("2024-01-01", "2024-01-31", CoverType.Yacht, 42556.25)] // Exactly 31 days, one day discount yacht
        [InlineData("2024-01-01", "2024-06-29", CoverType.BulkCarrier, 289201.25)] // Exactly 181 days, 150 days small discount, one day full discount default
        [InlineData("2024-01-01", "2024-06-29", CoverType.Yacht, 238452.500)] // Exactly 181 days, 150 days small discount, one day full discount yacht
        public void ComputePremium_ReturnsCorrectPremium(string startDateStr, string endDateStr, CoverType coverType, decimal expectedPremium)
        {
            // Arrange
            DateOnly startDate = DateOnly.Parse(startDateStr);
            DateOnly endDate = DateOnly.Parse(endDateStr);

            // Act
            decimal actualPremium = PremiumCalculator.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, actualPremium);
        }

    }
}
