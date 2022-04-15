using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Services;

namespace Sc3Hosted.Server.Data;

public class CompanyEntities : ApplicationDbContext
{
    public CompanyEntities(DbContextOptions options, IUserContextService userContextService) : base(options)
    {
        UserContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
    }

    public IUserContextService UserContextService { get; }

    public override  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessSave();
        return  base.SaveChangesAsync(cancellationToken);
    }

    private void ProcessSave()
    {
        var currentTime = DateTime.UtcNow;
        foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is BaseEntity))
        {
            var entity = item.Entity as BaseEntity;
            entity!.CreatedAt = currentTime;
            entity.CreatedBy = UserContextService.GetUserId;
            entity.UpdatedAt = currentTime;
            entity.UpdatedBy = UserContextService.GetUserId;
        }
        foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified && e.Entity is BaseEntity))
        {
            var entity = item.Entity as BaseEntity;
            entity!.UpdatedAt = currentTime;
            entity.UpdatedBy = UserContextService.GetUserId;
            item.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
            item.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
        }
    }
}
