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

        public PagedList<Product> GetProducts(ProductsParameters productsParams)
        {
            var products = GetAll().OrderBy(p => p.Id).AsQueryable();
            var productsOrdered = PagedList<Product>.ToPagedList(products,
                productsParams.PageNumber, productsParams.PageSize) ;

            return productsOrdered;
        }

        public IEnumerable<Product> GetProductsPerCategory(int id)
        {
            return GetAll().Where(x => x.CategoryId == id);
        }
    }
}
