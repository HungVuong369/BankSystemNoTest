using BankSystem.Dtos.Request;
using BankSystem.Repository;
using BankSystem.Service;
using BankSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Configuration;
using Mysqlx;
using System.Text.Json;

namespace BankSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly ILogger<BalanceController> _logger;

        [ActivatorUtilitiesConstructor]
        public BalanceController(IBalanceService balanceService, ILogger<BalanceController> logger)
        {
            _balanceService = balanceService;

            _logger = logger;
        }

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;

            _logger = HelperFunctions.CreateLogger<BalanceController>();
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpGet("getBalance/{accountNo}/{idCard}")]
        public IActionResult GetBalance(string accountNo, string idCard)
        {
            try
            {
                var result = _balanceService.GetBalance(accountNo, idCard);

                _logger.LogInformation("Get balance method called " + HelperFunctions.Instance.ConvertObjectToJson(result));

                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPost("holdAmount")]
        public IActionResult HoldAmount([FromBody] HoldAmountRequest holdAmountRequest)
        {
            try
            {
                var result = _balanceService.HoldAmount(holdAmountRequest.AccountNo, holdAmountRequest.IdCard, holdAmountRequest.Amount, holdAmountRequest.Description, holdAmountRequest.ApproveBy);

                _logger.LogInformation("Hold amount method called " + HelperFunctions.Instance.ConvertObjectToJson(result));

                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpPost("unholdAmount")]
        public IActionResult UnholdAmount([FromBody] UnholdAmount unholdAmount)
        {
            try
            {
                var result = _balanceService.UnHoldAmount(unholdAmount.AccountNo, unholdAmount.IdCard, unholdAmount.Amount, unholdAmount.Description, unholdAmount.ApproveBy);

                _logger.LogInformation("Unhold amount method called " + HelperFunctions.Instance.ConvertObjectToJson(result));

                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }
    }
}
