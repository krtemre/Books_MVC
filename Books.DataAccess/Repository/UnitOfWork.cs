using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;

namespace Books.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart{ get; private set; }


        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext dp)
        {
            _db = dp;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
