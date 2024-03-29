using BankSystem.Dtos.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BankSystem.Utilities
{
    public class HelperFunctions : Controller
    {
        public static int HttpStatusCodeOK = 200;
        public static int HttpStatusCodeNotFound = 404;
        public static int HttpStatusCodeBadRequest = 400;

        public static int ErrorCodeAccountNoUsed = 15;
        public static int ErrorCodeIdCardUsed = 16;
        public static int ErrorCodeWrongUsernameOrPassword = 17;
        public static int ErrorCodeNotFound = 1;
        public static int ErrorCodeIdCard_Does_Not_Belong_To_Account = 10;
        public static int ErrorCodeDoesNotHaveBalance = 11;
        public static int ErrorCodeAmountTakenExceed = 12;
        public static int ErrorCodeHoldAmountExceedUsable = 13;
        public static int ErrorCodeUnholdAmountExceedUsable = 14;
        public static int Success = 0;

        private static HelperFunctions _instance;

        private HelperFunctions() { }

        public static HelperFunctions Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HelperFunctions();
                }
                return _instance;
            }
        }

        public ResponseDto GetErrorResponseByError(int errorCode = 2)
        {
            var error = (Errors)errorCode;

            switch (error)
            {
                case Errors.No3:
                    return new ResponseDto(0, "Success");
                case Errors.No1:
                    return new ResponseDto(1, "No found data");
                case Errors.No2:
                    return new ResponseDto(2, "Error exeception");
                case Errors.No4:
                    return new ResponseDto(10, "Id card does not belong to this account");
                case Errors.No5:
                    return new ResponseDto(11, "Account does not have balance");
                case Errors.No6:
                    return new ResponseDto(12, "The amount taken exceeds the account balance");
                case Errors.No7:
                    return new ResponseDto(13, "The hold amount exceeds the usable balance");
                case Errors.No8:
                    return new ResponseDto(14, "The unhold amount exceeds the hold amount");
                case Errors.No9:
                    return new ResponseDto(15, "AccountNo is used, please cho other one");
                case Errors.No10:
                    return new ResponseDto(16, "ID card is used, please check again");
                case Errors.No11:
                    return new ResponseDto(17, "Wrong username or password");
                case Errors.No12:
                    return new ResponseDto(18, "Transaction has already been approved");
                case Errors.No13:
                    return new ResponseDto(19, "Unauthorized");
                case Errors.No14:
                    return new ResponseDto(20, "Token has not expired");
            }
            return null;
        }

        public IActionResult GetErrorResponseByError(ResponseDto response)
        {
            var error = (Errors)response.ErrorCode;

            switch (error)
            {
                case Errors.No3:
                    return Ok(response);
                case Errors.No1:
                    return NotFound(response);
                case Errors.No2:
                case Errors.No4:
                case Errors.No5:
                case Errors.No6:
                case Errors.No7:
                case Errors.No8:
                case Errors.No9:
                case Errors.No10:
                case Errors.No11:
                case Errors.No12:
                case Errors.No13:
                case Errors.No14:
                    return BadRequest(response);
            }
            return null;
        }

        public string ConvertObjectToJson(object obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public static ILogger<T> CreateLogger<T>()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            return loggerFactory.CreateLogger<T>();
        }
    }
}
