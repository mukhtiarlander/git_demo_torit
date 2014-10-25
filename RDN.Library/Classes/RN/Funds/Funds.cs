using RN.Library.DataModels.Context;
using RN.Library.DataModels.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RN.Library.Classes.Funds
{

    public class Fund
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public double TotalPaidToUser { get; set; }
        public double AmountToWithdraw { get; set; }
        public double ActiveInUserAccount { get; set; }
        public string BitCoinId { get; set; }
        public string PaypalAddress { get; set; }


        public static Fund GetCurrentFundsInformation(Guid userId)
        {
            Fund f = new Fund();

            var dc = new ManagementContext();
            var funds = dc.Funds.Where(x => x.UserId == userId).FirstOrDefault();
            if (funds == null)
            {
                f.ActiveInUserAccount = 0.00;
                f.AmountToWithdraw = 0.00;
                f.TotalPaidToUser = 0.00;
            }
            else
            {
                f.AmountToWithdraw = funds.ActiveInUserAccount;
                f.ActiveInUserAccount = funds.ActiveInUserAccount;
                f.TotalPaidToUser = funds.TotalPaidToUser;
                f.BitCoinId = funds.BitCoinId;
                f.PaypalAddress = funds.PaypalAddress;
            }
            f.UserId = userId;

            return f;
        }
        public static bool SaveCurrentPaypalBitcoinInformation(Fund fund)
        {

            var dc = new ManagementContext();
            var funds = dc.Funds.Where(x => x.UserId == fund.UserId).FirstOrDefault();
            if (funds == null)
            {
                FundsForWriter f = new FundsForWriter();
                f.BitCoinId = fund.BitCoinId;
                f.PaypalAddress = fund.PaypalAddress;
                f.UserId = fund.UserId;
                dc.Funds.Add(f);
            }
            else
            {
                funds.PaypalAddress = fund.PaypalAddress;
                funds.BitCoinId = fund.BitCoinId;
            }
            int c = dc.SaveChanges();

            return c > 0;
        }

    }
}
