namespace Jobtastic.Enums
{
    /// <summary>
    /// Why a posting is or is not on the public board. The stored IsOnline flag alone
    /// conflates three different reasons for being absent, which makes an overview
    /// hard to read.
    /// </summary>
    public enum PostingStatus
    {
        /// <summary>Never published - still being worked on.</summary>
        Draft,

        /// <summary>Published and within its runtime.</summary>
        Online,

        /// <summary>Was published, but its runtime has ended.</summary>
        Expired,

        /// <summary>Was published and taken down again while still valid.</summary>
        Offline
    }
}
