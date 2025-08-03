using PartnerTransactionAPI.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace PartnerTransactionAPI.Utils
{
    public static class SignatureUtil
    {
        public static bool IsBase64String(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                var decodedBytes = Convert.FromBase64String(input);
                var decodedStr = System.Text.Encoding.UTF8.GetString(decodedBytes);

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(decodedStr);
                var reEncoded = System.Convert.ToBase64String(plainTextBytes);

                // Strict check: re-encoded must equal original input
                return input.Equals(reEncoded, StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        public static string Base64Encode(string decodedText)
        {
            var decodedByte = System.Text.Encoding.UTF8.GetBytes(decodedText);
            return System.Convert.ToBase64String(decodedByte);
        }

        public static string Base64Decode(string encodedText)
        {
            var encodedByte = System.Convert.FromBase64String(encodedText);
            return System.Text.Encoding.UTF8.GetString(encodedByte);
        }

        public static string GenerateSignature(TransactionDto dto)
        {
            string formattedTimestamp = TimestampUtil.ConvertIsoToCustomFormat(dto.Timestamp, "yyyyMMddHHmmss");

            string rawString = $"{formattedTimestamp}{dto.PartnerKey}{dto.PartnerRefNo}{dto.TotalAmount}{dto.PartnerPassword}";

            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawString));

            string hexString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            string base64Signature = Convert.ToBase64String(Encoding.UTF8.GetBytes(hexString));

            return base64Signature;
        }

        public static bool ValidateSignature(TransactionDto dto)
        {
            {
                var expectedSig = GenerateSignature(dto);

                return string.Equals(expectedSig, dto.Signature, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
