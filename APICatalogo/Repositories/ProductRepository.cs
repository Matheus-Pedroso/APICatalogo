using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        

        public ProductRepository(APICatalogoContext context) : base(context) 
        {
        }

        public async Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams)
        {
            var products = await GetAllAsync();

            var productsOrdered = products.OrderBy(p => p.Id).AsQueryable();

            //var result = PagedList<Product>.ToPagedList(productsOrdered, productsParams.PageNumber, productsParams.PageSize) ;

            var result = await productsOrdered.ToPagedListAsync(productsParams.PageNumber, productsParams.PageSize);

            return result;
        }

        public async Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductsFilterPrice productPriceParams)
        {
            var products = await GetAllAsync();


            if (productPriceParams.Price.HasValue && !string.IsNullOrEmpty(productPriceParams.PriceCriterion))
            {
                if (productPriceParams.PriceCriterion.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price > productPriceParams.Price.Value).OrderBy(p => p.Price);
                }
                if (productPriceParams.PriceCriterion.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price < productPriceParams.Price.Value).OrderBy(p => p.Price);
                }
                if (productPriceParams.PriceCriterion.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price == productPriceParams.Price.Value).OrderBy(p => p.Name);
                }
            }

            //var productsFiltered = PagedList<Product>.ToPagedList(products.AsQueryable(), productPriceParams.PageNumber, productPriceParams.PageSize);

            var productsFiltered = await products.ToPagedListAsync(productPriceParams.PageNumber, productPriceParams.PageSize);

            return productsFiltered;
        }

        public async Task<IEnumerable<Product>> GetProductsPerCategoryAsync(int id)
        {
            var products = await GetAllAsync();
            var productsCategory = products.Where(x => x.CategoryId == id);
            return productsCategory;
        }
    }
}
