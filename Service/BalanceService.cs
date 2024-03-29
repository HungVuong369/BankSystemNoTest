using BankSystem.Data.Repositories;
using BankSystem.Dtos.Response;
using BankSystem.Repository;

namespace BankSystem.Service
{
    public class BalanceService : IBalanceService
    {
        private readonly BalanceRepository _balanceRepository;

        public BalanceService(BalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        public ResponseDto GetBalance(string accountNum, string idCard)
        {
            return _balanceRepository.GetBalance(accountNum, idCard);
        }

        public ResponseDto HoldAmount(string accountNo, string idCard, double amount, string descriptions, string approvedBy)
        {
            return _balanceRepository.HoldAmount(accountNo, idCard, amount, descriptions, approvedBy);
        }

        public ResponseDto UnHoldAmount(string accountNo, string idCard, double amount, string descriptions, string approvedBy)
        {
            return _balanceRepository.UnholdAmount(accountNo, idCard, amount, descriptions, approvedBy);
        }
    }
}
