using System.Linq.Expressions;

namespace APICatalogo.Repositories;

public interface IRepository<T>
{
    // Ao criar a interface repository generica, tomar cuidado para não violar o princípio ISP
    IEnumerable<T> GetAll();
    //T GetById(int id);
    T? GetById(Expression<Func<T, bool>> predicate);
    T Add(T entity);
    T Update(T entity);
    T Delete(T entity);
}
