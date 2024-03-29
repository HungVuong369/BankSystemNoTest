namespace BankSystem.Dtos.Response
{
    public class ResponseDto
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public ResponseDto(object data)
        {
            ErrorCode = 0;
            Message = "Success";
            Data = data;
        }

        public ResponseDto()
        {
            ErrorCode = 0;
            Message = "Success";
        }

        public ResponseDto(int code, string message)
        {
            ErrorCode = code;
            Message = message;
        }

        public ResponseDto(int code, string message, object data)
        {
            ErrorCode = code;
            Message = message;
            Data = data;
        }
    }
}
