namespace BankSystem.Repository
{
    public interface IRepository<T>
    {
        T GetById(object id);
        IEnumerable<T> GetAll();
        void Insert(T entity);
        void Update(T entity);
        void Delete(object id);
        void Save();
    }
}
