namespace Jobtastic.Models
{
    /// <summary>
    /// One row of the admin company overview. A company belongs to no single account -
    /// it is shared by everyone holding a mandate for it - so this is the only place
    /// where all companies can be seen at once.
    /// </summary>
    public class AdminCompanyListModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? WebsiteURL { get; set; }
        public int MandateHolderCount { get; set; }
        public int ContactCount { get; set; }
        public int PostingCount { get; set; }
    }
}
