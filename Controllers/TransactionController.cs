using BankSystem.Dtos.Request;
using BankSystem.Repository;
using BankSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IPdfService _ITransactionPdfService;
        private readonly ILogger<TransactionController> _logger;

        [ActivatorUtilitiesConstructor]
        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger, IPdfService iTransactionPdfService)
        {
            _transactionService = transactionService;
            _logger = logger;
            _ITransactionPdfService = iTransactionPdfService;
        }

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            _logger = HelperFunctions.CreateLogger<TransactionController>();
        }

        [HttpPost("depositMoney")]
        public IActionResult DepositMoney([FromBody] DepositRequest depositRequest)
        {
            try
            {
                var result = _transactionService.DepositMoney(depositRequest.AccountNo, depositRequest.Amount, depositRequest.Description);

                _logger.LogInformation("DepositMoney method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPost("depositApproval")]
        public IActionResult DepositApproval([FromBody] DepositApprovalRequest depositApprovalRequest)
        {
            try
            {
                var result = _transactionService.DepositApproval(depositApprovalRequest.TransactionId, depositApprovalRequest.Status, depositApprovalRequest.ApproveBy);

                _logger.LogInformation("DepositApproval method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [HttpPost("sellPayment")]
        public IActionResult SellPayment([FromBody] SellPaymentRequest sellPaymentRequest)
        {
            try
            {
                var result = _transactionService.SellPayment(sellPaymentRequest.AccountNo, sellPaymentRequest.IdCard, sellPaymentRequest.SecuritiesAccount, sellPaymentRequest.SecuritiesAccountIdCard, sellPaymentRequest.Amount, sellPaymentRequest.Descriptions);

                _logger.LogInformation("SellPayment method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [HttpPost("buyPayment")]
        public IActionResult BuyPayment([FromBody] BuyPaymentRequest buyPaymentRequest)
        {
            try
            {
                var result = _transactionService.BuyPayment(buyPaymentRequest.AccountNo, buyPaymentRequest.IdCard, buyPaymentRequest.SecuritiesAccount, buyPaymentRequest.SecuritiesAccountIdCard, buyPaymentRequest.Amount, buyPaymentRequest.Description);

                _logger.LogInformation("BuyPayment method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpGet("getTransactionPDF")]
        public IActionResult GetTransactionPDF()
        {
            try
            {
                byte[] pdfBytes = _ITransactionPdfService.GeneratePdf(UnitOfWork.Instance.TransactionRepo.Items);
                return File(pdfBytes, "application/pdf", "TransactionReport.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }
    }
}
