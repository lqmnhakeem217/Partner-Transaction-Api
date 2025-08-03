using System.Buffers.Text;
using System.Text;
using System;

namespace PartnerTransactionAPI.Utils
{
    public class StringUtil
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
    }


}

