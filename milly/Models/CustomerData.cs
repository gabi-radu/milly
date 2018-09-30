using System;
using System.Collections.Generic;

namespace BasicBot.Models
{
    public static class CustomerData
    {
        private static IDictionary<string, Mortgage> data = new Dictionary<string, Mortgage>
        {
            {
                "mike4mail@gmail.com",
                new Mortgage { Type = MortgageType.Variable, IsUpForRenewal = true, Balance = 123500M, InterestRate = 3.99M, Term = 23, Description = "SVR 3.99%" }
            },
        };

        public static Tuple<Mortgage, IEnumerable<Mortgage>> Find(string username)
        {
            var current = data[username];
            return new Tuple<Mortgage, IEnumerable<Mortgage>>(current, GetDeals(current));

        }

        private static IEnumerable<Mortgage> GetDeals(Mortgage currentMortgage)
        {
            if (currentMortgage.IsUpForRenewal)
            {
                return new[]
                {
                    new Mortgage
                    {
                        Balance = currentMortgage.Balance,
                        Term = currentMortgage.Term - 7,
                        IsUpForRenewal = false,
                        InterestRate = 1.35M,
                        Type = MortgageType.Fixed2Years,
                        Description = string.Format("Fixed 2 years 1.35%"),
                    }
                };
            }
            else
            {
                return null;
            }
        }
    }
}
