using System;

namespace VFL.Renderer.Services.Dashboard.Models
{
    public class DashboardResponse
    {

    }

    public class BalanceResponse
    {
        public bool isPostPay { get; set; }
        public bool isBroadband { get; set; }
        public float vodaStarRating { get; set; }
        public string currentPlan { get; set; }
        public Postpaybalances postpayBalances { get; set; }
        public Prepaybalance prepayBalance { get; set; }
    }

    public class Postpaybalances
    {
        public string unbilledAmount { get; set; }
        public string creditLimitAmount { get; set; }
        public Balance[] balances { get; set; }
    }
    public class Prepaybalance
    {
        public Balance[] balances { get; set; }
    }

    public class Balance
    {
        public string name { get; set; }
        public float balanceRemainingAmount { get; set; }
        public string balanceRemainingMeasurement { get; set; }
        public float totalAmount { get; set; }
        public string totalAmountMeasurement { get; set; }
        public string balanceType { get; set; }
        public DateTime expiry { get; set; }
    }

    public class DeviceResponse
    {
        public bool isRemoved { get; set; }
        public bool isSent { get; set; }        
        public bool isAdded { get; set; }
        public bool isCodeValid { get; set; }
        public string code { get; set; }        
    }


}
