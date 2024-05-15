using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        

        public ProductRepository(APICatalogoContext context) : base(context) 
        {
        }

        public IEnumerable<Product> GetProductsPerCategory(int id)
        {
            return GetAll().Where(x => x.CategoryId == id);
        }
    }
}
