namespace PartnerTransactionAPI.Utils
{
    public class TimestampUtil
    {
        public static string ConvertIsoToCustomFormat(string isoDateTime, string formatDate)
        {
            try
            {
                DateTime parsedDate = DateTime.Parse(isoDateTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                return parsedDate.ToString(formatDate);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool IsValid(string timestamp)
        {
            return DateTime.TryParse(timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out _);
        }

        public static bool IsExpired(string timestamp, int toleranceMinutes = 5)
        {
            if (!DateTime.TryParse(timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var requestTime))
            {
                return true;
            }

            var serverTime = DateTime.UtcNow;
            return requestTime < serverTime.AddMinutes(-toleranceMinutes) || requestTime > serverTime.AddMinutes(toleranceMinutes);
        }

        public static (bool IsValid, bool IsExpired) ValidateTimestamp(string timestamp, int toleranceMinutes = 5)
        {
            if (!DateTime.TryParse(timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var requestTime))
            {
                return (false, true);
            }

            var serverTime = DateTime.UtcNow;
            bool expired = requestTime < serverTime.AddMinutes(-toleranceMinutes);//|| requestTime > serverTime.AddMinutes(toleranceMinutes);
            return (true, expired);
        }
    }
}
