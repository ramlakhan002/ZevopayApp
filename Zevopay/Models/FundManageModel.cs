using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zevopay.Models
{
    public class FundManageModel
    {
        public List<SelectListItem> Users { get; set; }
        public string MemberId { get; set; }
        public string Factor { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public int TwoFactorCode { get; set;}
}
}
