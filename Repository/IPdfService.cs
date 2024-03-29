using BankSystem.Models;

namespace BankSystem.Repository
{
    public interface IPdfService
    {
        public byte[] GeneratePdf(List<Transaction> transactions);
    }
}
