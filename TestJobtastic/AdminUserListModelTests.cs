using Jobtastic.Models;

namespace TestJobtastic
{
    [TestFixture]
    public class AdminUserListModelTests
    {
        [Test]
        public void IsLocked_is_false_when_no_lockout_was_ever_set()
        {
            var user = new AdminUserListModel { LockoutEnd = null };

            Assert.That(user.IsLocked, Is.False);
        }

        [Test]
        public void IsLocked_is_false_when_the_lockout_has_already_elapsed()
        {
            // Identity leaves the old end date in place instead of clearing it, so
            // a plain null check would report every previously locked account as
            // still locked.
            var user = new AdminUserListModel { LockoutEnd = DateTimeOffset.Now.AddDays(-1) };

            Assert.That(user.IsLocked, Is.False);
        }

        [Test]
        public void IsLocked_is_true_while_the_lockout_end_is_still_in_the_future()
        {
            var user = new AdminUserListModel { LockoutEnd = DateTimeOffset.Now.AddDays(1) };

            Assert.That(user.IsLocked, Is.True);
        }
    }
}
