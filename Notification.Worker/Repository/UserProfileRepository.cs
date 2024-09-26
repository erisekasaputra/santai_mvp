using Microsoft.EntityFrameworkCore;
using Notification.Worker.Domain;
using Notification.Worker.Infrastructure;

namespace Notification.Worker.Repository;

public class UserProfileRepository
{
    private readonly NotificationDbContext _context;
    public UserProfileRepository(
        NotificationDbContext context)
    {
        _context = context; 
    }

    public async Task<UserProfile?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
    }

    public void Update(UserProfile userProfile)
    {
        _context.Users.Update(userProfile);
    }

    public async Task AddAsync(UserProfile userProfile)
    {
        await _context.Users.AddAsync(userProfile);
    }

    public async Task<IEnumerable<IdentityProfile>> GetProfiles(Guid userId)
    {
        var user = await _context.Users.AsNoTracking().Where(x => x.Id == userId).FirstOrDefaultAsync();

        return user is null ? [] : user.Profiles;  
    }
}
