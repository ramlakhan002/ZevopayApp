namespace Zevopay.Models
{
    public class MemberWalletTransactions
    {
        public decimal CrAmount { get; set; }
        public decimal DrAmount { get; set; }
        public decimal Balance { get; set; }
        public string MemberId { get; set; }
        public string Narration { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
