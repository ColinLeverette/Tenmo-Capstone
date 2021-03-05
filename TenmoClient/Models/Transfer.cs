using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public  class Transfer
    {
        public int TransferId { get; set; }
        public TransferType TransferTypeId { get; set; }
        public TransferStatus TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }

        public Transfer()
        {

        }


    }
}
