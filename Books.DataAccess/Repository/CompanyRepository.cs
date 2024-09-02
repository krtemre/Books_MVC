using Books.DataAccess.Data;
using Books.DataAccess.Repository.IRepository;
using Books.Models;

namespace Books.DataAccess.Repository
{
    internal class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Company obj)
        {
            _db.Companies.Update(obj);

            //var objFromDb = _db.Companies.FirstOrDefault(p => p.Id == obj.Id);

            //if (objFromDb != null)
            //{
            //    objFromDb.Name = obj.Name;
            //    objFromDb.StreetAddress = obj.StreetAddress;
            //    objFromDb.City = obj.City;
            //    objFromDb.State = obj.State;
            //    objFromDb.PostalCode = obj.PostalCode;
            //    objFromDb.PhoneNumber = obj.PhoneNumber;
            //}
        }
    }
}
