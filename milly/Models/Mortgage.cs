using System;

namespace BasicBot.Models
{
    public class Mortgage
    {
        public MortgageType Type { get; set; }

        public bool IsUpForRenewal { get; set; }

        public string Description { get; set; }

        public decimal Balance { get; set; }

        public decimal InterestRate { get; set; }

        public int Term { get; set; }

        public decimal MonthlyRepayment
        {
            get
            {
                return (decimal)((double)Balance * (double)InterestRate / 1200.0 * (Math.Pow(1 + ((double)InterestRate / 1200), Term * 12) / (Math.Pow(1 + ((double)InterestRate / 1200), Term * 12) - 1)));
            }
        }

        public decimal AnnualRepayment
        {
            get
            {
                return MonthlyRepayment * 12.0M;
            }
        }

        public decimal TotalRepayment
        {
            get
            {
                return AnnualRepayment * Term;
            }
        }
    }
}
