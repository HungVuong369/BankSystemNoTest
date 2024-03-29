using BankSystem.Dtos.Response;

namespace BankSystem.Repository
{
    public interface ICustomerService
    {
        ResponseDto OpenCustomer(string accountNo, string idCard, string name, DateTime dateOfBirth, string address, string phoneNumber, string cardPlace, byte typeId, long userId);
        ResponseDto GetAccount(string accountNo, string idCard);
    }
}
