using Claims.Auditing;
using Microsoft.EntityFrameworkCore;

public class AuditContext : DbContext
{
    public AuditContext(DbContextOptions<AuditContext> options) : base(options) {}
     
    public DbSet<ClaimAudit> ClaimAudits { get; set; }
    public DbSet<CoverAudit> CoverAudits { get; set; }
}

