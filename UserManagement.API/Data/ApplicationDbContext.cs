using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Models.Entities;
using UserManagement.API.Models.Interfaces;

namespace UserManagement.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(r => r.Description).HasMaxLength(256);
                entity.HasQueryFilter(r => !r.IsDeleted);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries())
            {
                // 1. Inline Audit Tracking (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedAt = DateTime.UtcNow;
                        if (string.IsNullOrEmpty(auditable.CreatedBy))
                        {
                            auditable.CreatedBy = userId;
                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.UpdatedAt = DateTime.UtcNow;
                        auditable.UpdatedBy = userId;
                    }
                }

                // 2. Soft Delete Processing
                if (entry.Entity is ISoftDelete softDelete && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = DateTime.UtcNow;
                    softDelete.DeletedBy = userId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
