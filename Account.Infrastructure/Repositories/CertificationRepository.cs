using Account.Domain.Aggregates.CertificationAggregate;
using Account.Domain.Aggregates.UserAggregate;
using Account.Domain.Enumerations;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Repositories;

public class CertificationRepository : ICertificationRepository
{
    private readonly AccountDbContext _context;

    public CertificationRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<Certification?> CreateAsync(Certification certification)
    {
        var entity = await _context.Certifications.AddAsync(certification);
        return entity.Entity;
    }

    public void Delete(Certification certification)
    {
        _context.Certifications.Remove(certification);
    } 

    public async Task<bool> GetAnyByCertificationIdAsync(string id)
    {
        return await _context.Certifications
          .AsNoTracking().AnyAsync(x => x.CertificationId == id);
    }

    public async Task<bool> GetAnyByIdAsync(Guid id)
    {
        return await _context.Certifications
            .AsNoTracking().AnyAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Certification>?> GetByCertIdsAsExcludingIdsNoTrackingAsync(params (Guid, string)[] identities)
    {
        var predicate = PredicateBuilder.New<Certification>(true);

        foreach((Guid idExcluder, string certificateId) in identities)
        {
            predicate = predicate.Or(x => x.Id != idExcluder && x.CertificationId == certificateId);
        }

        return await _context.Certifications
            .AsNoTracking().Where(predicate)
            .AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Certification>?> GetByCertIdsAsNoTrackingAsync(params string[] id)
    {
        return await _context.Certifications
            .Where(x => id.Contains(x.CertificationId))
            .AsNoTracking().ToListAsync();
    }

    public async Task<Certification?> GetByCertificationIdAsync(string id)
    {
        return await _context.Certifications.FirstOrDefaultAsync(x => x.CertificationId == id);
    }

    public async Task<Certification?> GetByIdAsync(Guid id)
    {
        return await _context.Certifications.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(int TotalCount, int TotalPages, IEnumerable<Certification> Certifications)> GetPaginatedCertificationByUserId(Guid userId, int pageNumber, int pageSize)
    {
        var query = _context.Certifications.AsQueryable();

        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking() 
            .Where(x => x.MechanicUserId == userId)
            .OrderBy(x => x.CertificationName)
            .ToListAsync();

        return (totalCount, totalPages, items);
    }

    public void Update(Certification certification)
    {
        _context.Certifications.Update(certification);
    }
}
