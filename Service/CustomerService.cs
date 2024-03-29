using BankSystem.Data.Repositories;
using BankSystem.Dtos.Response;
using BankSystem.Repository;

namespace BankSystem.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerService(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public ResponseDto OpenCustomer(string accountNo, string idCard, string name, DateTime dateOfBirth, string address, string phoneNumber, string cardPlace, byte typeId, long userId)
        {
            return _customerRepository.OpenCustomer(accountNo, idCard, name, dateOfBirth, address, phoneNumber, cardPlace, typeId, userId);
        }

        public ResponseDto GetAccount(string accountNo, string idCard)
        {
            return _customerRepository.GetAccount(accountNo, idCard);
        }
    }
}
