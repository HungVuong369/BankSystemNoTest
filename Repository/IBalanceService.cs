using BankSystem.Dtos.Response;

namespace BankSystem.Repository
{
    public interface IBalanceService
    {
        ResponseDto GetBalance(string accountNum, string idCard);
        ResponseDto HoldAmount(string accountNo, string idCard, double amount, string descriptions, string approvedBy);
        ResponseDto UnHoldAmount(string accountNo, string idCard, double amount, string descriptions, string approvedBy);
    }
}
