using Jobtastic.Models;
using System.Linq.Expressions;

namespace Jobtastic.Authorization
{

    /// <summary>
    /// Central visibility rules for job postings.
    /// Each query on postings should run through one of these scopes, so that a
    /// rule is defined in only one place.
    /// </summary>
    public static class PostingQueries
    {
        /// <summary>
        /// Postings that the current user is allowed to manage: their own, for admins all.
        /// </summary>
        public static IQueryable<JobPosting> ManageableBy(this IQueryable<JobPosting> postings, ICurrentUser me)
        {
            if (me.IsAdmin)
                return postings;

            if (!me.IsAuthenticated || me.Id is null)
                return postings.Where(p => false);

            return postings.Where(p => p.OwnerID == me.Id);
        }
        /// <summary>
        /// Publicly visible postings: published and not yet expired.
        /// Applies to both the job portal and the public API.
        /// </summary>
        private static readonly Expression<Func<JobPosting, bool>> PubliclyVisibleRule =
            p => p.IsOnline && p.ExpiryDate > DateTime.Now;

        private static readonly Func<JobPosting, bool> PubliclyVisibleCheck = PubliclyVisibleRule.Compile();

        public static IQueryable<JobPosting> PubliclyVisible(this IQueryable<JobPosting> postings) =>
            postings.Where(PubliclyVisibleRule);

        /// <summary>
        /// Publicly visible postings: published and not yet expired.
        /// Applied to a posting that has already been loaded.
        /// </summary>
        public static bool IsPubliclyVisible(this JobPosting posting) => PubliclyVisibleCheck(posting);

        /// <summary>
        /// Postings still flagged online whose expiry date has passed.
        /// </summary>
        public static IQueryable<JobPosting> Expired(this IQueryable<JobPosting> postings) =>
            postings.Where(p => p.IsOnline && p.ExpiryDate <= DateTime.Now);
    }
}
