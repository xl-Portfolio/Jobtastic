using Jobtastic.Authorization;
using Jobtastic.Models;

namespace TestJobtastic
{
    [TestFixture]
    public class PostingQueriesTests
    {
        private class FakeCurrentUser : ICurrentUser
        {
            public string? Id { get; init; }
            public bool IsAdmin { get; init; }
            public bool IsAuthenticated => Id is not null;
        }

        private static IQueryable<JobPosting> Postings() => new[]
        {
            new JobPosting { ID = 1, OwnerID = "alice", IsOnline = true, ExpiryDate = DateTime.Now.AddMonths(1) },
            new JobPosting { ID = 2, OwnerID = "bob", IsOnline = true, ExpiryDate = DateTime.Now.AddMonths(1) },
            new JobPosting { ID = 3, OwnerID = "alice", IsOnline = false, ExpiryDate = DateTime.Now.AddMonths(1) },
            new JobPosting { ID = 4, OwnerID = "bob", IsOnline = true, ExpiryDate = DateTime.Now.AddDays(-1) },
        }.AsQueryable();

        [Test]
        public void ManageableBy_Admin_sees_every_posting()
        {
            var admin = new FakeCurrentUser { Id = "carol", IsAdmin = true };

            var result = Postings().ManageableBy(admin).Select(p => p.ID);

            Assert.That(result, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void ManageableBy_RegularUser_sees_only_their_own()
        {
            var alice = new FakeCurrentUser { Id = "alice", IsAdmin = false };

            var result = Postings().ManageableBy(alice).Select(p => p.ID);

            Assert.That(result, Is.EquivalentTo(new[] { 1, 3 }));
        }

        [Test]
        public void ManageableBy_AnonymousUser_sees_nothing()
        {
            // Regression guard: OwnerID == null must not fall through to "sees
            // ownerless postings" for an unauthenticated caller.
            var anonymous = new FakeCurrentUser { Id = null, IsAdmin = false };

            var result = Postings().ManageableBy(anonymous);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PubliclyVisible_excludes_offline_and_expired_postings()
        {
            var result = Postings().PubliclyVisible().Select(p => p.ID);

            Assert.That(result, Is.EquivalentTo(new[] { 1, 2 }));
        }
    }
}
