using PartnerTransactionAPI.DTOs;

namespace PartnerTransactionAPI.Services
{
    public class DiscountService
    {
        public static DiscountItemDto CalculateDiscount(long totalAmountInCents)
        {
            decimal totalAmountMYR = totalAmountInCents / 100m; // Convert cents to MYR
            decimal discountPercentage = 0;

            if (totalAmountMYR >= 200 && totalAmountMYR <= 500)
                discountPercentage += 5;
            else if (totalAmountMYR >= 501 && totalAmountMYR <= 800)
                discountPercentage += 7;
            else if (totalAmountMYR >= 801 && totalAmountMYR <= 1200)
                discountPercentage += 10;
            else if (totalAmountMYR > 1200)
                discountPercentage += 15;

            if (totalAmountMYR > 500 && IsPrime((int)totalAmountMYR))
                discountPercentage += 8;

            if (totalAmountMYR > 900 && totalAmountMYR % 10 == 5)
                discountPercentage += 10;


            if (discountPercentage > 20)
                discountPercentage = 20;


            decimal discountAmountMYR = totalAmountMYR * (discountPercentage / 100);
            decimal finalAmountMYR = totalAmountMYR - discountAmountMYR;


            long discountAmountCents = (long)Math.Round(discountAmountMYR * 100);
            long finalAmountCents = (long)Math.Round(finalAmountMYR * 100);

            return new DiscountItemDto
            {
                OriginalAmount = totalAmountInCents,
                DiscountPercentage = discountPercentage,
                DiscountAmount = discountAmountCents,
                FinalAmount = finalAmountCents
            };
        }

        private static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i <= Math.Sqrt(number); i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}
