using APICatalogo.Models;
using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(APICatalogoContext context) : base(context)
    {
    }

    public PagedList<Category> GetCategories(CategoriesParameters parameters)
    {
        
        var category = GetAll().OrderBy(p => p.Id).AsQueryable();

        var categoryOrdered = PagedList<Category>.ToPagedList(category,
            parameters.PageNumber, parameters.PageSize);

        return categoryOrdered;
        
    }
}
