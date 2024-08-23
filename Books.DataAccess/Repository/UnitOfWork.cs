using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;

namespace Books.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext dp)
        {
            _db = dp;
            Category = new CategoryRepository(dp);
            Product = new ProductRepository(dp);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
