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
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<JobPosting>() //Company weg -> Postings weg
				.HasOne(j => j.Company)
				.WithMany(c => c.Postings)
				.HasForeignKey(j => j.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Owner)
                .WithMany(u => u.Postings)
                .HasForeignKey(j => j.OwnerID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Contact)
                .WithMany(c => c.Postings)
                .HasForeignKey(j => j.ContactID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<JobContact>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Contacts)
                .HasForeignKey(c => c.CompanyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<JobContact>() //1 Kontakt gehört zu einem User. 1 User kann (zeitweise) mehrere Kontaktseiten betreuen
                .HasOne(j => j.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(j => j.UserID)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }


}
