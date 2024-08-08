using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Helper;

public class MetaTableHelper(AccountDbContext context)
{
    private readonly AccountDbContext _context = context;

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
