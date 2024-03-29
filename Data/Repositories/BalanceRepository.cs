using BankSystem.Dtos.Response;
using BankSystem.Utilities;
using Dapper;

namespace BankSystem.Data.Repositories
{
    public class BalanceRepository
    {
        private readonly IDataAccess _dataAccess;

        public BalanceRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public ResponseDto GetBalance(string accountNum, string idCard)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@accountNo", accountNum);
            parameters.Add("@idCard", idCard);

            var check = _dataAccess.ExecuteStoredProcedure<int>("isBalanceValid", parameters);

            var responseDto = HelperFunctions.Instance.GetErrorResponseByError(check);

            if (responseDto != null && responseDto.ErrorCode != 0)
                return responseDto;

            parameters = new DynamicParameters();
            parameters.Add("@accountNum", accountNum);
            parameters.Add("@idCard", idCard);
            responseDto.Data = _dataAccess.ExecuteStoredProcedure<BalanceTotal>("getBalance", parameters);

            return responseDto;
        }

        public ResponseDto HoldAmount(string accountNo, string idCard, double amount, string descriptions, string approveBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@accountNo", accountNo);
            parameters.Add("@idCard", idCard);
            parameters.Add("@amount", amount);
            parameters.Add("@descriptions", descriptions);
            parameters.Add("@approveBy", approveBy);

            var check = _dataAccess.ExecuteStoredProcedure<int>("holdAmount", parameters);

            if (check == 0)
            {
                parameters = new DynamicParameters();
                parameters.Add("@accountNum", accountNo);
                parameters.Add("@idCard", idCard);
                return new ResponseDto(_dataAccess.ExecuteStoredProcedure<BalanceTotal>("getBalance", parameters));
            }
            return HelperFunctions.Instance.GetErrorResponseByError(check);
        }

        public ResponseDto UnholdAmount(string accountNo, string idCard, double amount, string descriptions, string approveBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@accountNo", accountNo);
            parameters.Add("@idCard", idCard);
            parameters.Add("@amount", amount);
            parameters.Add("@descriptions", descriptions);
            parameters.Add("@approveBy", approveBy);

            var check = _dataAccess.ExecuteStoredProcedure<int>("unholdAmount", parameters);

            if (check == 0)
            {
                parameters = new DynamicParameters();
                parameters.Add("@accountNum", accountNo);
                parameters.Add("@idCard", idCard);
                return new ResponseDto(_dataAccess.ExecuteStoredProcedure<BalanceTotal>("getBalance", parameters));
            }
            return HelperFunctions.Instance.GetErrorResponseByError(check);
        }
    }
}
