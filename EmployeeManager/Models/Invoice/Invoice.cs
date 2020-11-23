using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models.Invoice
{
    public class Invoice
    {
        public int Id { get; set; }
        public Vendor Vendor { get; set; }
        public Cart Cart { get; set; }
        public Customer Customer { get; set; }
        public PaymentInfo paymentInfo { get; set; }
    }
}
