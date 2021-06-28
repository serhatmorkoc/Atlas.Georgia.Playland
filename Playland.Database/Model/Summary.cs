using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playland.Database.Model
{
    public class Summary
    {
        public decimal TotalTransaction { get; set; }
        public int TotalFreeGame { get; set; }
        public int TotalGameCount { get; set; }
        public decimal TotalGameAmount { get; set; }

        public decimal TotalCashAmount { get; set; }

        public decimal TotalCreditCard { get; set; }

        public decimal TotalBonus { get; set; }

        public List<CardPlayCount> CardPlays { get; set; }

        public int TotalUploadQuantity { get; set; }

        public int TotalVisitor { get; set; }
    }

    public class CardPlayCount
    {
        public string CardNo { get; set; }
        public int PlayCount { get; set; }
    }
}
