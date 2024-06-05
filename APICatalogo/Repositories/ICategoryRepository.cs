using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters parameters);

        Task<IPagedList<Category>> GetCategoryFilterNameAsync(CategoriesFilterName categoriesParams);
    }
}
