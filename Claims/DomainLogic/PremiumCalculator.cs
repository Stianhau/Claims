using Claims;

public static class PremiumCalculator
{

    public static decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        int baseDayRate = 1250;

        var coverTypeToMultiplier = new Dictionary<CoverType, decimal>{
            { CoverType.Yacht, 1.1m },
            { CoverType.PassengerShip, 1.2m },
            { CoverType.Tanker, 1.5m }
        };
        decimal defaultMultiplier = 1.3m;

        decimal multiplier = coverTypeToMultiplier.ContainsKey(coverType) ? coverTypeToMultiplier[coverType] : defaultMultiplier;

        decimal premiumPerDay = baseDayRate * multiplier;

        // Expects that when the startDate and endDate is the same, insuranceLength will be 1 day
        decimal insuranceLength = endDate.DayNumber - startDate.DayNumber + 1;

        decimal discountNext150Days = coverType == CoverType.Yacht ? 0.05m : 0.02m;
        decimal discountNext150DaysFactor = 1 - discountNext150Days;
        
        decimal discountRemainingDays = coverType == CoverType.Yacht ? 0.08m : 0.03m;
        decimal discountRemainingDaysFactor = 1 - discountRemainingDays;

        decimal priceFirst30Days = Math.Min(insuranceLength, 30) * premiumPerDay;
        if (insuranceLength <= 30)
        {
            return priceFirst30Days;
        }

        decimal remainingDays = insuranceLength - 30;
        decimal priceNext150Days = Math.Min(remainingDays, 150) * premiumPerDay * discountNext150DaysFactor;
        if (insuranceLength <= 180)
        {
            return priceFirst30Days + priceNext150Days;
        }

        remainingDays -= 150;
        decimal priceRemainingDays = remainingDays * premiumPerDay * discountRemainingDaysFactor;
        return priceFirst30Days + priceNext150Days + priceRemainingDays;
    }
}
