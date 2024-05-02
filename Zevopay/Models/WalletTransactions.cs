namespace Zevopay.Models
{
    public class WalletTransactions
    {
        public int WalletTransactionID { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Factor { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime AddDate { get; set; }
    }
}
