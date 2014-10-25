using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Payment.Paywall;
using Scoreboard.Library.ViewModel;

namespace RDN.Models.OutModel
{
    public class GameOutModel
    {
        public GameViewModel Game { get; set; }
        public Paywall Paywall { get; set; }
        public string StripeKey { get; set; }
        
    }
}