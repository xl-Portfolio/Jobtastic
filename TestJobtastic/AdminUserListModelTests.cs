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

        [Test]
        public void IsAdmin_reflects_whether_the_Admin_role_is_present()
        {
            var admin = new AdminUserListModel { Roles = { "User", "Admin" } };
            var plain = new AdminUserListModel { Roles = { "User" } };

            Assert.That(admin.IsAdmin, Is.True);
            Assert.That(plain.IsAdmin, Is.False);
        }

        [Test]
        public void EffectiveRole_reports_the_highest_role_held()
        {
            // Roles are stored additively, so an owner also holds Admin and User.
            // Only the top of that stack is shown.
            var owner = new AdminUserListModel { Roles = { "User", "Admin", "Owner" } };
            var admin = new AdminUserListModel { Roles = { "User", "Admin" } };
            var plain = new AdminUserListModel { Roles = { "User" } };

            Assert.That(owner.EffectiveRole, Is.EqualTo("Owner"));
            Assert.That(admin.EffectiveRole, Is.EqualTo("Admin"));
            Assert.That(plain.EffectiveRole, Is.EqualTo("User"));
        }

        [Test]
        public void EffectiveRole_falls_back_to_User_when_no_role_is_recorded()
        {
            // Legacy rows from before roles were additive can hold no baseline role;
            // the overview should still show something sensible rather than a blank.
            var roleless = new AdminUserListModel();

            Assert.That(roleless.EffectiveRole, Is.EqualTo("User"));
        }
    }
}
