using System.Linq.Expressions;

namespace APICatalogo.Repositories;

public interface IRepository<T>
{
    // Ao criar a interface repository generica, tomar cuidado para não violar o princípio ISP
    Task<IEnumerable<T>> GetAllAsync();
    //T GetById(int id);
    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate);
    T Add(T entity);
    T Update(T entity);
    T Delete(T entity);  
}
