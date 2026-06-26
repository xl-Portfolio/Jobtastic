using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Jobtastic.Models;

namespace Jobtastic.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public DbSet<JobPosting> JobPostings { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<CompanyContact> CompanyContacts { get; set; }
		public DbSet<JobApplicant> Applicants { get; set; }
		public DbSet<Application> Applications { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Restrict für alle:
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .Where(e => e.ClrType.Namespace == "Jobtastic.Models")
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

			modelBuilder.Entity<JobPosting>()
				.HasOne(j => j.Company)
				.WithMany(c => c.Postings)
				.HasForeignKey(j => j.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Contact)
                .WithMany(c => c.Postings)
                .HasForeignKey(j => j.ContactID);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Posting)
                .WithMany(p => p.Applications)
                .HasForeignKey(a => a.PostingID);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Applicant)
                .WithMany(a => a.Applications)
                .HasForeignKey(a => a.ApplicantID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyContact>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Contacts)
                .HasForeignKey(c => c.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }


}
