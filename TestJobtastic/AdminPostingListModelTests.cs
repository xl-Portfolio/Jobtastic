using Jobtastic.Enums;
using Jobtastic.Models;

namespace TestJobtastic
{
    [TestFixture]
    public class AdminPostingListModelTests
    {
        private static AdminPostingListModel Posting(bool isOnline, DateTime uploadDate, DateTime expiryDate) =>
            new() { IsOnline = isOnline, UploadDate = uploadDate, ExpiryDate = expiryDate };

        [Test]
        public void A_posting_that_was_never_published_is_a_draft()
        {
            // Both dates are only stamped on publishing, so an unpublished posting has
            // an expiry of default(DateTime) - which lies in the past and would read as
            // expired if the upload date were not checked first.
            var draft = Posting(isOnline: false, uploadDate: default, expiryDate: default);

            Assert.That(draft.Status, Is.EqualTo(PostingStatus.Draft));
        }

        [Test]
        public void A_published_posting_within_its_runtime_is_online()
        {
            var live = Posting(isOnline: true, uploadDate: DateTime.Now.AddDays(-5), expiryDate: DateTime.Now.AddMonths(2));

            Assert.That(live.Status, Is.EqualTo(PostingStatus.Online));
        }

        [Test]
        public void A_posting_past_its_expiry_is_expired_regardless_of_the_online_flag()
        {
            // The sweep clears IsOnline lazily, so both combinations occur in the data:
            // expired-but-still-flagged, and expired-and-already-cleared.
            var notSweptYet = Posting(isOnline: true, uploadDate: DateTime.Now.AddMonths(-5), expiryDate: DateTime.Now.AddDays(-1));
            var swept = Posting(isOnline: false, uploadDate: DateTime.Now.AddMonths(-5), expiryDate: DateTime.Now.AddDays(-1));

            Assert.That(notSweptYet.Status, Is.EqualTo(PostingStatus.Expired));
            Assert.That(swept.Status, Is.EqualTo(PostingStatus.Expired));
        }

        [Test]
        public void A_published_posting_taken_down_while_still_valid_is_offline()
        {
            // The only state a person caused deliberately - which is why it is worth
            // distinguishing from an expiry.
            var takenDown = Posting(isOnline: false, uploadDate: DateTime.Now.AddDays(-5), expiryDate: DateTime.Now.AddMonths(2));

            Assert.That(takenDown.Status, Is.EqualTo(PostingStatus.Offline));
        }
    }
}
