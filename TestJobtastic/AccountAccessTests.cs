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
    }
}
