using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Context;

public class APICatalogoContext : DbContext
{
    public APICatalogoContext(DbContextOptions<APICatalogoContext> options) : base(options)
    {        
    }

    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }
}
