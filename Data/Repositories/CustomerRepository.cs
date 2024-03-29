using BankSystem.Dtos.Response;
using BankSystem.Utilities;
using Dapper;

namespace BankSystem.Data.Repositories
{
    public class CustomerRepository
    {
        private readonly IDataAccess _dataAccess;

        public CustomerRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public ResponseDto OpenCustomer(string accountNo, string idCard, string name, DateTime dateOfBirth, string address, string phoneNumber, string cardPlace, byte typeId, long userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@accountNo", accountNo);
            parameters.Add("@idCard", idCard);
            parameters.Add("@name", name);
            parameters.Add("@dateOfBirth", dateOfBirth);
            parameters.Add("@address", address);
            parameters.Add("@phoneNumber", phoneNumber);
            parameters.Add("@cardPlace", cardPlace);
            parameters.Add("@typeId", typeId);
            parameters.Add("@userId", userId);

            var check = _dataAccess.ExecuteStoredProcedure<int>("openCustomer", parameters);

            return HelperFunctions.Instance.GetErrorResponseByError(check);
        }

        public ResponseDto GetAccount(string accountNo, string idCard)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@accountNo", accountNo);
            parameters.Add("@idCard", idCard);

            var check = _dataAccess.ExecuteStoredProcedure<int>("isIdCardBelongToAccount", parameters);

            var responseDto = HelperFunctions.Instance.GetErrorResponseByError(check);

            if (responseDto != null && responseDto.ErrorCode != 0)
                return responseDto;

            return new ResponseDto(_dataAccess.ExecuteStoredProcedure<AccountInfo>("getAccount", parameters));
        }
    }
}
