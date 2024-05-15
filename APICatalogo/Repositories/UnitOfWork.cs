using APICatalogo.Context;

namespace APICatalogo.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProductRepository? _productRepo;
    private ICategoryRepository? _categoryRepo;

    public APICatalogoContext _context;

    public UnitOfWork(APICatalogoContext context)
    {
        _context = context;
    }

    public IProductRepository ProductRepository
    {
        get
        {
            return _productRepo = _productRepo ?? new ProductRepository(_context);
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepo = _categoryRepo ?? new CategoryRepository(_context);
        }
    }
    public void Commit()
    {
        _context.SaveChanges();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
