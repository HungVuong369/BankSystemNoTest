using BankSystem.Data.Repositories;
using BankSystem.Dtos.Response;
using BankSystem.Repository;

namespace BankSystem.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionRepository _transactionRepository;

        public TransactionService(TransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public ResponseDto BuyPayment(string accountNo, string idCard, string securitiesAccount, string securitiesAccountIdCard, double amount, string descriptions)
        {
            return _transactionRepository.BuyPayment(accountNo, idCard, securitiesAccount, securitiesAccountIdCard, amount, descriptions);
        }

        public ResponseDto DepositApproval(string transactionId, int status, string approveBy)
        {
            return _transactionRepository.DepositApproval(transactionId, status, approveBy);
        }

        public ResponseDto DepositMoney(string accountNo, double amount, string description)
        {
            return _transactionRepository.DepositMoney(accountNo, amount, description);
        }

        public ResponseDto SellPayment(string accountNo, string idCard, string securitiesAccount, string securitiesAccountIdCard, double amount, string descriptions)
        {
            return _transactionRepository.SellPayment(accountNo, idCard, securitiesAccount, securitiesAccountIdCard, amount, descriptions);
        }
    }
}
