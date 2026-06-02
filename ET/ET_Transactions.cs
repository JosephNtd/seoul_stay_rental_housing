using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class ET_Transactions
    {
        public long ID { get; set; }
        public Guid GUID { get; set; }
        public long UserID { get; set; }
        public long TransactionTypeID { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string GatewayReturnID { get; set; }
        public string TransactionTypeName { get; set; }

        public string StatusName { get; set; }

        public string Description { get; set; }

        public ET_Transactions() { }

        public ET_Transactions(long iD, Guid gUID, long userID, long transactionTypeID, decimal amount, DateTime transactionDate, string gatewayReturnID)
        {
            ID = iD;
            GUID = gUID;
            UserID = userID;
            TransactionTypeID = transactionTypeID;
            Amount = amount;
            TransactionDate = transactionDate;
            GatewayReturnID = gatewayReturnID;
        }
    }
}
