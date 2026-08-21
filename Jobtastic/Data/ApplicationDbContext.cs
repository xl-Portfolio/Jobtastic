using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Jobtastic.Models;

namespace Jobtastic.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public DbSet<JobPosting> Postings { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<JobContact> Contacts { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Companies)
                .WithMany(c => c.Users);

            modelBuilder.Entity<JobPosting>()
				.HasOne(j => j.Company)
				.WithMany(c => c.Postings)
				.HasForeignKey(j => j.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Owner)
                .WithMany(u => u.Postings)
                .HasForeignKey(j => j.OwnerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Contact)
                .WithMany(c => c.Postings)
                .HasForeignKey(j => j.ContactID)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<JobContact>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Contacts)
                .HasForeignKey(c => c.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobContact>() 
                .HasOne(j => j.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(j => j.UserID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }


}
