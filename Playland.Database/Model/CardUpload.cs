using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playland.Database.Model
{
    public class CardUpload
    {
        public string CardUID { get; set; }
        public bool IsNewCard { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }


        private List<CardTransaction> _transactions;
        public List<CardTransaction> Transactions
        {
            get
            {
                if (_transactions == null)
                {
                    _transactions = new List<CardTransaction>();
                }
                return _transactions;
            }
            set { _transactions = value; }
        }
    }
}
