using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProducts(ProductsParameters productsParams);
        IEnumerable<Product> GetProductsPerCategory(int id);
    }
}
