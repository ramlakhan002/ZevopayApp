namespace Zevopay.Models
{
    public class MoneyTransferModel
    {
        public string? PaymentMode { get; set; }
        public long AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string? FullName { get; set; }
        public decimal Amount { get; set; }
    }
}
