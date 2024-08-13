using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Vehicle.API.Domain.Entities;
using Vehicle.API.Infrastructures.Repositories;

namespace Vehicle.API.Infrastructures;


public class UnitOfWork : IUnitOfWork
{
    private readonly VehicleDbContext _context;

    private IDbContextTransaction? _transaction;
     
    public IFleetRepository Fleets { get; }

    public UnitOfWork(VehicleDbContext context)
    {
        _context = context;  
        Fleets = new FleetRepository(context); 
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("No transaction started.");
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _transaction.CommitAsync(cancellationToken);

            _transaction.Dispose();

            _transaction = null;
        }
        catch (Exception)
        {
            throw;
        }
    } 

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction started.");
        }

        await _transaction.RollbackAsync(cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
