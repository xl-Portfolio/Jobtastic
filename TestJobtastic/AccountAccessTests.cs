using Jobtastic.Authorization;

namespace TestJobtastic
{
    [TestFixture]
    public class AccountAccessTests
    {
        private const string Caller = "caller-id";
        private const string Other = "other-id";

        [Test]
        public void Without_a_requested_id_the_caller_targets_their_own_account()
        {
            Assert.That(AccountAccess.MayTarget(Caller, null, isAdmin: false), Is.True);
            Assert.That(AccountAccess.MayTarget(Caller, "", isAdmin: false), Is.True);
        }

        [Test]
        public void Requesting_ones_own_id_explicitly_is_allowed()
        {
            Assert.That(AccountAccess.MayTarget(Caller, Caller, isAdmin: false), Is.True);
        }

        [Test]
        public void A_non_admin_may_not_target_someone_elses_account()
        {
            // This is the case that has to produce 403 - the rule the whole
            // admin-on-behalf-of feature rests on.
            Assert.That(AccountAccess.MayTarget(Caller, Other, isAdmin: false), Is.False);
        }

        [Test]
        public void An_admin_may_target_someone_elses_account()
        {
            Assert.That(AccountAccess.MayTarget(Caller, Other, isAdmin: true), Is.True);
        }

        [Test]
        public void An_admin_may_not_lock_their_own_account()
        {
            // A self-lock could not be lifted by the account that caused it.
            Assert.That(AccountAccess.MayLock(Caller, Caller, targetIsOwner: false), Is.False);
        }

        [Test]
        public void An_admin_may_lock_another_account()
        {
            Assert.That(AccountAccess.MayLock(Caller, Other, targetIsOwner: false), Is.True);
        }

        [Test]
        public void An_admin_may_not_revoke_their_own_admin_role()
        {
            // Keeps at least one admin in the system: whoever performs the action
            // always retains their own role.
            Assert.That(AccountAccess.MayRevokeAdmin(Caller, Caller, targetIsOwner: false), Is.False);
        }

        [Test]
        public void An_admin_may_revoke_another_accounts_admin_role()
        {
            Assert.That(AccountAccess.MayRevokeAdmin(Caller, Other, targetIsOwner: false), Is.True);
        }

        [Test]
        public void The_owner_can_neither_be_locked_nor_demoted_by_another_admin()
        {
            // The threat this guards against is not the system running out of admins,
            // but an admin - possibly one the owner promoted - deposing the owner.
            Assert.That(AccountAccess.MayLock(Caller, Other, targetIsOwner: true), Is.False);
            Assert.That(AccountAccess.MayRevokeAdmin(Caller, Other, targetIsOwner: true), Is.False);
        }

        [Test]
        public void A_missing_target_id_is_never_actionable()
        {
            Assert.That(AccountAccess.MayLock(Caller, null, targetIsOwner: false), Is.False);
            Assert.That(AccountAccess.MayRevokeAdmin(Caller, "", targetIsOwner: false), Is.False);
        }
    }
}
