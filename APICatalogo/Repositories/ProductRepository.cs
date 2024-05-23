using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        

        public ProductRepository(APICatalogoContext context) : base(context) 
        {
        }

        public IEnumerable<Product> GetProducts(ProductsParameters productsParams)
        {
            return GetAll()
                .OrderBy(p => p.Name)
                .Skip((productsParams.PageNumber - 1) * productsParams.PageSize)
                .Take(productsParams.PageSize).ToList();
        }

        public IEnumerable<Product> GetProductsPerCategory(int id)
        {
            return GetAll().Where(x => x.CategoryId == id);
        }
    }
}
