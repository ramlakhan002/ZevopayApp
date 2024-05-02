namespace Zevopay.Data.Entity
{
    public class Surcharge
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }
        public IEnumerable<Packages> Packages { get; set; }
        public string PackageName { get; set; }
        public decimal RangeFrom { get; set; }
        public decimal RangeTo { get; set; }
        public decimal SurchargeAmount { get; set; }
        public bool IsFlat { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }

    public class SurchargeModel
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal RangeFrom { get; set; }
        public decimal RangeTo { get; set; }
        public decimal SurchargeAmount { get; set; }
        public bool IsFlat { get; set; }
    }
}
