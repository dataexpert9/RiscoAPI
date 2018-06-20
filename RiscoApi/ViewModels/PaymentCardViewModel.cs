using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class PaymentCardViewModel
    {
        public int Id { get; set; }
        
        public string CardNumber { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        public string CCV { get; set; }
        
        public string NameOnCard { get; set; }
        
        public int CardType { get; set; } //1 for Credit, 2 for Debit

        public int User_ID { get; set; }
    }
}