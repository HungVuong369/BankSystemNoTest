using BankSystem.Dtos.Response;

namespace BankSystem.Repository
{
    public interface ITransactionService
    {
        ResponseDto DepositMoney(string accountNo, double amount, string description);
        ResponseDto DepositApproval(string transactionId, int status, string approvedBy);
        ResponseDto SellPayment(string accountNo, string idCard, string securitiesAccount, string securitiesAccountIdCard, double amount, string descriptions);
        ResponseDto BuyPayment(string accountNo, string idCard, string securitiesAccount, string securitiesAccountIdCard, double amount, string descriptions);
    }
}
