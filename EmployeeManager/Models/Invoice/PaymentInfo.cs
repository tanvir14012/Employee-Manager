using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.Invoice
{
    public class PaymentInfo
    {
        public int CustomerId { get; set; }
        [Display(Name = "Card Type")]
        public string CardType { get; set; }
        [Display(Name = "Card No")]
        public string CardNo { get; set; }
        [Display(Name = "Transaction Type")]
        public string Trxtype { get; set; }
        [Display(Name = "Merchant Bank Id")]
        public string MerchantBankId { get; set; }
        [Display(Name = "Merchant Bank Uid")]
        public string MerchantBankUid { get; set; }
        [Display(Name = "Bank Approved Code")]
        public string BankApprovedCode { get; set; }
        public string IpAddress { get; set; }
        [Display(Name = "Card Statement Show")]
        public string CardStatementShow { get; set; }
        [Display(Name = "Gateway Currency")]
        public string GatewayCurrency { get; set; }
        [Display(Name = "Amount Paid")]
        public double? AmountPaid { get; set; }
        public int CartId { get; set; } 

    }
}
