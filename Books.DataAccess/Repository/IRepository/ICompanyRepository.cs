using Books.Models;

namespace Books.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);
        void Save();
    }
}
