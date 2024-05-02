namespace Zevopay.Models
{
    public class WalletModel
    {
        public string Id { get; set; }
        public string MemberId { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Balance { get; set; }
        public string Narration { get; set; }
        public string MemberName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
