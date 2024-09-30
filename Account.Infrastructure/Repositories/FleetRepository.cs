using Account.Domain.Aggregates.FleetAggregate; 
using Account.Domain.Enumerations;
using Account.Infrastructure.Helper;
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore; 

namespace Account.Infrastructure.Repositories;

public class FleetRepository : IFleetRepository
{
    private readonly AccountDbContext _context;

    private readonly MetaTableHelper _metaTableHelper;

    public FleetRepository(AccountDbContext accountDbContext)
    {
        _context = accountDbContext;
        _metaTableHelper = new MetaTableHelper(accountDbContext);
    }

    public async Task<Fleet> CreateAsync(Fleet fleet)
    { 
        var entry = await _context.Fleets.AddAsync(fleet);
        return entry.Entity;
    }

    public void Delete(Fleet fleet)
    {
        _context.Fleets.Remove(fleet);
    }

    public async Task DeleteByUserId(Guid id)
    {
        var tableName = _metaTableHelper.GetTableName<Fleet>();
        var fieldDelete = _metaTableHelper.GetColumnName<Fleet>(nameof(Fleet.UserId));
         
        var query = $"DELETE FROM [{tableName}] WHERE [{fieldDelete}] = @UserId";

        // Execute the query with parameters
        await _context.Database.ExecuteSqlRawAsync(
            query, new SqlParameter("@UserId", id));
    }

    public async Task<bool> GetAnyByIdentityAsync(params (FleetLegalParameter parameter, string hashedValue)[] clauses)
    {
        var predicate = PredicateBuilder.New<Fleet>();

        foreach ((FleetLegalParameter parameter, string hashedValue) in clauses)
        {
            switch (parameter)
            {
                case FleetLegalParameter.ChassisNumber:
                    predicate = predicate.Or(x => x.HashedChassisNumber == hashedValue);
                    break;
                case FleetLegalParameter.EngineNumber:
                    predicate = predicate.Or(x => x.HashedEngineNumber == hashedValue);
                    break;
                case FleetLegalParameter.RegistrationNumber:
                    predicate = predicate.Or(x => x.HashedRegistrationNumber == hashedValue);
                    break; 
            }
        }

        return await _context.Fleets.Where(predicate)
            .AsNoTracking()
            .AnyAsync();
    }

    public async Task<Fleet?> GetByIdentityAsync(params (FleetLegalParameter parameter, string hashedValue)[] clauses)
    {
        var predicate = PredicateBuilder.New<Fleet>();

        foreach ((FleetLegalParameter parameter, string hashedValue) in clauses)
        {
            switch (parameter)
            {
                case FleetLegalParameter.ChassisNumber:
                    predicate = predicate.Or(x => x.HashedChassisNumber == hashedValue);
                    break;
                case FleetLegalParameter.EngineNumber:
                    predicate = predicate.Or(x => x.HashedEngineNumber == hashedValue);
                    break;
                case FleetLegalParameter.RegistrationNumber:
                    predicate = predicate.Or(x => x.HashedRegistrationNumber == hashedValue);
                    break;
            }
        }

        return await _context.Fleets.Where(predicate)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<Fleet?> GetByIdentityExcludingUserIdAsync(
        Guid userId, 
        params (FleetLegalParameter parameter, string hashedValue)[] clauses)
    {
        var predicate = PredicateBuilder.New<Fleet>();

        foreach ((FleetLegalParameter parameter, string hashedValue) in clauses)
        {
            switch (parameter)
            {
                case FleetLegalParameter.ChassisNumber:
                    predicate = predicate.Or(x => x.HashedChassisNumber == hashedValue);
                    break;
                case FleetLegalParameter.EngineNumber:
                    predicate = predicate.Or(x => x.HashedEngineNumber == hashedValue);
                    break;
                case FleetLegalParameter.RegistrationNumber:
                    predicate = predicate.Or(x => x.HashedRegistrationNumber == hashedValue);
                    break;
            }
        }

        return await _context.Fleets.Where(predicate)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId != userId );
    }

    public async Task<IEnumerable<Fleet>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Fleets.Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Fleet>> GetByStaffIdAsync(Guid staffId)
    { 
        return await _context.Fleets.Where(x => x.StaffId == staffId).ToListAsync();    
    }

    public async Task<IEnumerable<Fleet>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Fleets.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<Fleet?> GetByUserIdAndIdAsync(Guid userId, Guid fleetId)
    {
        return await _context.Fleets
            .Where(x => 
                (x.UserId == userId) 
                && x.Id == fleetId)
            .FirstOrDefaultAsync();
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Fleet> Fleets)> GetPaginatedFleetByUserId(
        Guid userId,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Fleets.AsQueryable();
        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Where(x => x.UserId == userId) // Apply the filter first
            .OrderBy(x => x.UserId)         // Apply OrderBy after Where
            .ThenBy(x => x.StaffId)
            .ThenBy(x => x.Brand)
            .ThenBy(x => x.Model)
            .Skip((pageNumber - 1) * pageSize)  // Pagination applied after filtering and ordering
            .Take(pageSize)
            .AsNoTracking() // AsNoTracking can be applied after pagination and sorting
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public void Update(Fleet fleet)
    {
        _context.Fleets.Update(fleet);
    }

    public void UpdateRange(IEnumerable<Fleet> fleets)
    {
        _context.Fleets.UpdateRange(fleets);    
    }
}
