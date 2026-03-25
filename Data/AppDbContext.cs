using Microsoft.EntityFrameworkCore;
using CaseInvestigationManagementSystem.Models;

namespace CaseInvestigationManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<AuditTrailModel> Audits { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<CaseModel> Cases { get; set; }
    }
}