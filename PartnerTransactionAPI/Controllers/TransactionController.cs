using Microsoft.AspNetCore.Mvc;
using PartnerTransactionAPI.Constants;
using PartnerTransactionAPI.DTOs;
using PartnerTransactionAPI.Services;
using PartnerTransactionAPI.Utils;

namespace PartnerTransactionAPI.Controllers
{
    //[ApiController]
    [Route("api")]
    public class TransactionController : ControllerBase
    {
        private readonly List<string> AllowedPartners = new() { "FAKEGOOGLE", "FAKEPEOPLE" };

        [HttpPost("submittrxmessage")]
        public IActionResult SubmitTransaction([FromBody] TransactionDto request)
        {
            try
            {
                LogUtil.LogRequest(request);

                #region Model Validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                    LogUtil.LogError(ApiConstant.ErrorCause.ValidFailure, 400, new Exception(string.Join(", ", errors)), request);

                    return BadRequest(new ReceiptDto
                    {
                        Result = 0,
                        ResultMessage = string.Join(" ", errors)
                    });
                }

                #endregion

                #region Check PartnerPassword encoded or not
                if (!SignatureUtil.IsBase64String(request.PartnerPassword))
                {
                    var encodedValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.PartnerPassword));
                    request.PartnerPassword = encodedValue;
                }

                #endregion

                #region Partner Validation/Signature MisMatch
                bool isValidSignature = SignatureUtil.ValidateSignature(request);

                if (!AllowedPartners.Contains(request.PartnerKey) || !isValidSignature)
                {
                    string cause = !AllowedPartners.Contains(request.PartnerKey)
                        ? ApiConstant.ErrorMessage.PartnerKeyNotAllow
                        : ApiConstant.ErrorMessage.SigMisMatch;

                    LogUtil.LogError(ApiConstant.ErrorCause.PKeySigMisMatch, 400, new Exception(cause), request);
                    return BadRequest(new ReceiptDto
                    {
                        Result = (int)ApiConstant.Status.Error,
                        ResultMessage = ApiConstant.ErrorMessage.AccessDenied
                    });
                }

                #endregion

                #region TimeStamp Validation
                var (isValid, isExpired) = TimestampUtil.ValidateTimestamp(request.Timestamp);

                if (!isValid)
                {
                    LogUtil.LogError(ApiConstant.ErrorCause.InvalidTimeStamp, 400, new Exception(ApiConstant.ErrorMessage.InvalidTimestamp), request);
                    return BadRequest(new ReceiptDto
                    {
                        Result = (int)ApiConstant.Status.Error,
                        ResultMessage = ApiConstant.ErrorMessage.InvalidTimestamp
                    });
                }

                if (isExpired)
                {
                    LogUtil.LogError(ApiConstant.ErrorCause.TimestampExpired, 400, new Exception(ApiConstant.ErrorMessage.TimeStampExpired), request);
                    return BadRequest(new ReceiptDto
                    {
                        Result = (int)ApiConstant.Status.Error,
                        ResultMessage = ApiConstant.ErrorMessage.TimeStampExpired
                    });
                }

                #endregion

                #region ItemDetails == TotalAmout Validation
                if (request.Items != null && request.Items.Count > 0)
                {
                    long sumItems = request.Items.Sum(obj => obj.UnitPrice * obj.Quantity);

                    if (sumItems != request.TotalAmount)
                    {
                        LogUtil.LogError(ApiConstant.ErrorCause.InvalidAmount, 400, new Exception(ApiConstant.ErrorMessage.InvalidAmount), request);
                        return StatusCode(StatusCodes.Status400BadRequest,new ReceiptDto
                        {
                            Result = (int)ApiConstant.Status.Error,
                            ResultMessage = ApiConstant.ErrorMessage.InvalidAmount
                        });
                    }
                }

                #endregion

                var discountResult = DiscountService.CalculateDiscount(request.TotalAmount);
                LogUtil.LogResponse(request);
                return StatusCode(StatusCodes.Status200OK, new ReceiptDto
                {
                    Result = (int)ApiConstant.Status.Success,
                    TotalAmount = discountResult.OriginalAmount,
                    TotalDiscount = discountResult.DiscountAmount,
                    FinalAmount = discountResult.FinalAmount
                });
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ApiConstant.ErrorCause.InServerError, 500, ex, request);
                return StatusCode(500, ApiConstant.ErrorMessage.InServerError);
            }
        }
    }
}
