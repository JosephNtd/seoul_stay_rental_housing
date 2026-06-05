using System;

namespace ET
{
    public class ET_Invoices
    {
        public long ID { get; set; }

        public Guid GUID { get; set; } = Guid.NewGuid();

        public long BookingID { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Pending | Paid | Cancelled | Expired
        /// </summary>
        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        public DateTime? PaidDate { get; set; }

        public string Notes { get; set; }
    }
}