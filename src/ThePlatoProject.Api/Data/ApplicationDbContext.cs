using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace MinimalAPI2026Demo.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<ArtifactMediaFile> MediaFiles { get; set; }
        public DbSet<CatalogRecord> CatalogRecords { get; set; }
        public DbSet<CatalogRecordNote> CatalogNotes { get; set; }

        public DbSet<DirectMessage> DirectMessages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureApplicationUserRelationships(builder);
            ConfigureSiteRelationships(builder);
            ConfigureArtifactRelationships(builder);
            ConfigureMediaRelationships(builder);
            ConfigureCatalogRelationships(builder);
            ConfigureMessageRelationships(builder);
            ConfigureAuditRelationships(builder);
            ConfigureEnumConversions(builder);
        }
        private static void ConfigureApplicationUserRelationships(
            ModelBuilder builder)
        {
            // Each employee may be assigned to at most one Site.
            // Each Site may have multiple assigned employees.
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.AssignedSite)
                .WithMany(s => s.AssignedEmployees)
                .HasForeignKey(u => u.AssignedSiteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Preserve the Admin who deactivated an employee.
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.DeactivatedBy)
                .WithMany()
                .HasForeignKey(u => u.DeactivatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Preserve the Admin who restored an employee.
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.RestoredBy)
                .WithMany()
                .HasForeignKey(u => u.RestoredById)
                .OnDelete(DeleteBehavior.Restrict);
        }
        private static void ConfigureSiteRelationships(ModelBuilder builder)
        {
            builder.Entity<Site>()
                .HasOne(s => s.VerifiedBy)
                .WithMany()
                .HasForeignKey(s => s.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Site>()
                .HasOne(s => s.ClosureRequestedBy)
                .WithMany()
                .HasForeignKey(s => s.ClosureRequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Site>()
                .HasOne(s => s.ClosureAuthorizedBy)
                .WithMany()
                .HasForeignKey(s => s.ClosureAuthorizedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Site>()
                .HasOne(s => s.ClosedBy)
                .WithMany()
                .HasForeignKey(s => s.ClosedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
        private static void ConfigureArtifactRelationships(
           ModelBuilder builder)
        {
            // Preserve Artifacts when a Site is closed.
            builder.Entity<Artifact>()
                .HasOne(a => a.Site)
                .WithMany(s => s.Artifacts)
                .HasForeignKey(a => a.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Artifact>()
                .HasOne(a => a.VerifiedBy)
                .WithMany()
                .HasForeignKey(a => a.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Artifact>()
                .HasOne(a => a.TransferRequestedBy)
                .WithMany()
                .HasForeignKey(a => a.TransferRequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Artifact>()
                .HasOne(a => a.TransferAuthorizedBy)
                .WithMany()
                .HasForeignKey(a => a.TransferAuthorizedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Artifact>()
                .HasOne(a => a.ShipmentSentBy)
                .WithMany()
                .HasForeignKey(a => a.ShipmentSentById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Artifact>()
                .HasOne(a => a.ShipmentReceivedBy)
                .WithMany()
                .HasForeignKey(a => a.ShipmentReceivedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureMediaRelationships(ModelBuilder builder)
        {
            // Media remains attached to its Artifact.
            builder.Entity<ArtifactMediaFile>()
                .HasOne(m => m.Artifact)
                .WithMany(a => a.MediaFiles)
                .HasForeignKey(m => m.ArtifactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ArtifactMediaFile>()
                .HasOne(m => m.UploadedBy)
                .WithMany(u => u.UploadedMedia)
                .HasForeignKey(m => m.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ArtifactMediaFile>()
                .HasOne(m => m.VerifiedBy)
                .WithMany()
                .HasForeignKey(m => m.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureCatalogRelationships(
            ModelBuilder builder)
        {
            // Catalog Records remain attached to their Artifact.
            builder.Entity<CatalogRecord>()
                .HasOne(cr => cr.Artifact)
                .WithMany(a => a.CatalogRecords)
                .HasForeignKey(cr => cr.ArtifactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CatalogRecord>()
                .HasOne(cr => cr.SubmittedBy)
                .WithMany(u => u.SubmittedCatalogRecords)
                .HasForeignKey(cr => cr.SubmittedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Notes remain attached to their Catalog Record.
            builder.Entity<CatalogRecordNote>()
                .HasOne(n => n.CatalogRecord)
                .WithMany(cr => cr.Notes)
                .HasForeignKey(n => n.CatalogRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CatalogRecordNote>()
                .HasOne(n => n.Author)
                .WithMany(u => u.AuthoredCatalogNotes)
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureMessageRelationships(
            ModelBuilder builder)
        {
            builder.Entity<DirectMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DirectMessage>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureAuditRelationships(ModelBuilder builder)
        {
            builder.Entity<AuditLog>()
                .HasOne(a => a.ActorUser)
                .WithMany(u => u.AuditEvents)
                .HasForeignKey(a => a.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureEnumConversions(ModelBuilder builder)
        {
            builder.Entity<Artifact>()
                .Property(a => a.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<Artifact>()
                .Property(a => a.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<Artifact>()
                .Property(a => a.CustodyStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<ArtifactMediaFile>()
                .Property(m => m.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<Site>()
                .Property(s => s.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Entity<Site>()
                .Property(s => s.LifecycleStatus)
                .HasConversion<string>()
                .HasMaxLength(50);
        }

    }
    }
