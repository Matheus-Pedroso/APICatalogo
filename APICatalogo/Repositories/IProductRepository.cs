using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams);
        Task<IEnumerable<Product>> GetProductsPerCategoryAsync(int id);

        Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductsFilterPrice priceParams);
    }
}
