namespace Jobtastic.Models
{
    /// <summary>
    /// One row of the admin-exclusive company overview.
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
