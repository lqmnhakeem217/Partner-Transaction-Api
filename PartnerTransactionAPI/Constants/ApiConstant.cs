namespace PartnerTransactionAPI.Constants
{
    public class ApiConstant
    {
        public class ErrorMessage
        {
            public const string AccessDenied = "Access Denied!";
            public const string TimeStampExpired = "Timestamp expired beyond allowed tolerance.";
            public const string InvalidTimestamp = "Invalid timestamp format.";
            public const string InvalidAmount = "Invalid total amount.";
            public const string SigMisMatch = "Signature mismatch.";
            public const string PartnerKeyNotAllow = "Unauthorized partner key.";
            public const string InServerError = "Internal server error.";
            public const string SerializeFailed = "Failed to serialize request for error logging.";
        }

        public class ErrorCause
        {
            public const string ValidFailure = "Message{Object} Validation Failed!";
            public const string PKeySigMisMatch = $"{ErrorMessage.PartnerKeyNotAllow}/{ErrorMessage.SigMisMatch}";
            public const string InvalidTimeStamp = "Invalid timestamp format.";
            public const string TimestampExpired = "Timestamp expired beyond allowed tolerance.";
            public const string InvalidAmount = "Invalid total amount.";
            public const string InServerError = "Internal server error.";
        }

        public enum Status
        {
            Error,
            Success,
        }
    }
}
