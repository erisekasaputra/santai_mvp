namespace Account.Domain.Aggregates.CertificationAggregate;

public interface ICertificationRepository
{ 
    Task<(int TotalCount, int TotalPages, IEnumerable<Certification> Certifications)> GetPaginatedCertificationByUserId(Guid userId, int pageNumber, int pageSize);

    Task<IEnumerable<Certification>?> GetByCertIdsAsNoTrackingAsync(params string[] id);

    Task<IEnumerable<Certification>?> GetByCertIdsAsExcludingIdsNoTrackingAsync(params (Guid, string)[] identities);

    Task<Certification?> GetByCertificationIdAsync(string id);

    Task<bool> GetAnyByCertificationIdAsync(string id);

    Task<Certification?> GetByIdAsync(Guid id);

    Task<bool> GetAnyByIdAsync(Guid id);

    Task<Certification?> CreateAsync(Certification certification);
     
    void Update(Certification certification);
    
    void Delete(Certification certification);
}
