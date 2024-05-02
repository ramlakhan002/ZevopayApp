//namespace Zevopay.Models
//{

//    public class BankAccountMoneyTransRequestModel
//    {
//        public string name { get; set; }
//        public string ifsc { get; set; }
//        public string account_number { get; set; }
//    }

//    public class ContactMoneyTransRequestModel
//    {
//        public string name { get; set; }
//        public string email { get; set; }
//        public string contact { get; set; }
//        public string type { get; set; }
//        public string reference_id { get; set; }
//        public NotesMoneyTransRequestModel notes { get; set; }
//    }

//    public class FundAccountMoneyTransRequestModel
//    {
//        public string account_type { get; set; }
//        public BankAccountMoneyTransRequestModel bank_account { get; set; }
//        public ContactMoneyTransRequestModel contact { get; set; }
//    }

//    public class NotesMoneyTransRequestModel
//    {
//        public string notes_key_1 { get; set; }
//        public string notes_key_2 { get; set; }
//    }

//    public class PayoutsMoneyTransferRequestModel
//    {
//        public string account_number { get; set; }
//        public int amount { get; set; }
//        public string currency { get; set; }
//        public string mode { get; set; }
//        public string purpose { get; set; }
//        public FundAccountMoneyTransRequestModel fund_account { get; set; }
//        public bool queue_if_low_balance { get; set; }
//        public string reference_id { get; set; }
//        public string narration { get; set; }
//        public NotesMoneyTransRequestModel notes { get; set; }
//    }


//    //public class PayoutsModel
//    //{
//    //}
//}
