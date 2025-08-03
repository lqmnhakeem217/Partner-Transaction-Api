using log4net;
using PartnerTransactionAPI.Constants;
using System.Text.Json;

namespace PartnerTransactionAPI.Utils
{
    public static class LogUtil
    {
        #region Log File
        private static readonly ILog requestLogger = LogManager.GetLogger("RequestLogger");
        private static readonly ILog responseLogger = LogManager.GetLogger("ResponseLogger");
        private static readonly ILog errorLogger = LogManager.GetLogger("ErrorLogger");

        public static void LogRequest(object request)
        {
            try
            {
                string json = JsonSerializer.Serialize(request);
                json = MaskPsLog(json);
                requestLogger.Info($"[REQUEST] {DateTime.UtcNow:O}\nPayload: {json}\n");
            }
            catch (Exception ex)
            {
                errorLogger.Warn($"Failed to log request: {ex.Message}");
            }
        }

        public static void LogResponse(object response)
        {
            try
            {
                string json = JsonSerializer.Serialize(response);
                json = MaskPsLog(json);
                responseLogger.Info($"[RESPONSE] {DateTime.UtcNow:O}\nPayload: {json}\n");
            }
            catch (Exception ex)
            {
                errorLogger.Warn($"Failed to log response: {ex.Message}");
            }
        }

        public static void LogError(string cause, int statusCode, Exception ex, object? request = null)
        {
            string requestJson = string.Empty;
            if (request != null)
            {
                try
                {
                    requestJson = JsonSerializer.Serialize(request);
                    requestJson = MaskPsLog(requestJson);
                }
                catch
                {
                    requestJson = $"[{ApiConstant.ErrorMessage.SerializeFailed}]";
                }
            }

            errorLogger.Error($"[ERROR] {DateTime.UtcNow:O}\nStatus: {statusCode}\nCause: {cause}\nRequest: {requestJson}\nException: {ex}\n");
        }

        #endregion

        private static string MaskPsLog(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (dict == null || dict.Count == 0)
                return json;

            foreach (var key in dict.Keys.ToList())
            {
                if (key.Equals("partnerpassword", StringComparison.OrdinalIgnoreCase))
                {
                    var value = dict[key].GetString();
                    if (!string.IsNullOrEmpty(value) && !SignatureUtil.IsBase64String(value))
                    {
                        var encodedValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
                        dict[key] = JsonSerializer.Deserialize<JsonElement>($"\"{encodedValue}\"");
                    }
                }
            }

            return JsonSerializer.Serialize(dict);
        }

    }
}
