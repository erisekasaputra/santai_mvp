using Master.Data.API.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Master.Data.API.Infrastructure;

public class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options)
{
    public DbSet<Banner> Banners { get; set; }
}
