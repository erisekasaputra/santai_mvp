using System.Data;
using Vehicle.API.Domain.Entities;

namespace Vehicle.API.Infrastructures;

public interface IUnitOfWork
{
    IFleetRepository Fleets { get; } 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default); 
}
