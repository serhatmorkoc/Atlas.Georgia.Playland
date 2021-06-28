using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playland.Database.Model
{
    public class CardTransaction
    {
        public decimal Amount { get; set; }
        public int TransactionTypeId { get; set; }

        public string CreationDate { get; set; }
    }
}
