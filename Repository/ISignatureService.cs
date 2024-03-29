namespace BankSystem.Repository
{
    public interface ISignatureService
    {
        MemoryStream Signature(byte[] existingPdfStream, string password = null);
    }
}
