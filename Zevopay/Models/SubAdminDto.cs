namespace Zevopay.Models
{
    public class SubAdminDto
    {
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalCount { get; set; }
        public string SearchText { get; set; }
        public int TotalRecord { get; set; }
        public List<SubAdminModel> SubAdminData { get; set; }

    }
}
