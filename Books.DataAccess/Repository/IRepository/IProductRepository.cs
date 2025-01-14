﻿using Books.Models;

namespace Books.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product category);
        void Save();
    }
}
