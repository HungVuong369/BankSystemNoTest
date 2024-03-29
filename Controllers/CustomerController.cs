using BankSystem.Dtos.Request;
using BankSystem.Repository;
using BankSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        [ActivatorUtilitiesConstructor]
        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
            _logger = HelperFunctions.CreateLogger<CustomerController>();
        }

        [HttpPost("openCustomer")]
        public IActionResult OpenCustomer([FromBody] OpenCustomerRequest openCustomerRequest)
        {
            try
            {
                var result = _customerService.OpenCustomer(openCustomerRequest.AccountNo, openCustomerRequest.IdCard, openCustomerRequest.Name, openCustomerRequest.DateOfBirth, openCustomerRequest.Address, openCustomerRequest.PhoneNumber, openCustomerRequest.CardPlace, openCustomerRequest.TypeId, openCustomerRequest.UserId);
                
                _logger.LogInformation("OpenCustomer method called " + HelperFunctions.Instance.ConvertObjectToJson(result));
                return HelperFunctions.Instance.GetErrorResponseByError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, HelperFunctions.Instance.GetErrorResponseByError());
            }
        }

        [Authorize(Roles = "ROLE_ADMIN")]
        [HttpGet("getAccount/{accountNo}/{idCard}")]
        public IActionResult GetAccount(string accountNo, string idCard)
        {
            try
            {
                var result = _customerService.GetAccount(accountNo, idCard);

                _logger.LogInformation("GetAccount method called " + HelperFunctions.Instance.ConvertObjectToJson(result));

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
