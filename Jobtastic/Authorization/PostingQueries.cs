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
        /// Defined once as an expression so the SQL filter and the in-memory
        /// check below cannot drift apart.
        /// </summary>
        private static readonly Expression<Func<JobPosting, bool>> PubliclyVisibleRule =
            p => p.IsOnline && p.ExpiryDate > DateTime.Now;

        private static readonly Func<JobPosting, bool> PubliclyVisibleCheck = PubliclyVisibleRule.Compile();

        public static IQueryable<JobPosting> PubliclyVisible(this IQueryable<JobPosting> postings) =>
            postings.Where(PubliclyVisibleRule);

        /// <summary>
        /// Same rule, applied to a posting that has already been loaded.
        /// </summary>
        public static bool IsPubliclyVisible(this JobPosting posting) => PubliclyVisibleCheck(posting);
    }
}
