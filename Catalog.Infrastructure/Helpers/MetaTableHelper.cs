using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Helpers;

public class MetaTableHelper(CatalogDbContext context)
{
    private readonly CatalogDbContext _context = context;

    public string? GetTableName<TEntity>() where TEntity : class
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity)); 
        return entityType?.GetTableName();
    }

    public string? GetColumnName<TEntity>(string propertyName) where TEntity : class
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var property = entityType?.FindProperty(propertyName);
        return property?.GetColumnName(StoreObjectIdentifier.Table(entityType?.GetTableName()!, entityType?.GetSchema()));
    }
}
