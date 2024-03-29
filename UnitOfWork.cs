using BankSystem.Data;
using BankSystem.Data.Repositories;
using BankSystem.Models;
using BankSystem.Utilities;
using iText.Commons.Bouncycastle.Security;

namespace BankSystem
{
    public class UnitOfWork
    {
        private static UnitOfWork _instance;

        private TransactionRepository _transactionRepository;
        private IDataAccess dataAccess = new DataAccess();

        public TransactionRepository TransactionRepo
        {
            get { 
                if(_transactionRepository == null)
                {
                    _transactionRepository = new TransactionRepository(dataAccess);
                }
                return _transactionRepository; 
            }
            set { _transactionRepository = value; }
        }

        private UnitOfWork() 
        {

        }

        public static UnitOfWork Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UnitOfWork();
                }
                return _instance;
            }
        }
    }
}
