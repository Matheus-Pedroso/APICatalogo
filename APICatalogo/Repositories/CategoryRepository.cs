using APICatalogo.Models;
using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Pagination;
using System.Linq;
using X.PagedList;

namespace APICatalogo.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(APICatalogoContext context) : base(context)
    {
    }

    public async Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams)
    {

        var category = await GetAllAsync();

        var categoryOrdered = category.OrderBy(c => c.Name).AsQueryable();

        //var result = PagedList<Category>.ToPagedList(categoryOrdered, parameters.PageNumber, parameters.PageSize);

        var result = await categoryOrdered.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);

        return result;
         
    }

    public async Task<IPagedList<Category>> GetCategoryFilterNameAsync(CategoriesFilterName categoriesParams)
    {
        var categories = await GetAllAsync(); // código assincrono


        if (!string.IsNullOrEmpty(categoriesParams.Name))
        {
            categories = categories.Where(c => c.Name.Contains(categoriesParams.Name, StringComparison.OrdinalIgnoreCase));
        }

        //var categoriesFiltered = PagedList<Category>.ToPagedList(categories.AsQueryable(), categoriesParams.PageNumber, categoriesParams.PageSize);

        var categoriesFiltered = await categories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);

        return categoriesFiltered;
    }


}
