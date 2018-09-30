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
                new Mortgage { Type = MortgageType.Variable, IsUpForRenewal = true, Balance = 183503.12M, InterestRate = 7, Term = 23, Description = "SVR 7%" }
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
                        Term = currentMortgage.Term,
                        IsUpForRenewal = false,
                        InterestRate = currentMortgage.InterestRate - 2,
                        Type = MortgageType.Fixed5Years,
                        Description = string.Format("Fixed 5 years {0}%", currentMortgage.InterestRate - 2),
                    },
                    new Mortgage
                    {
                        Balance = currentMortgage.Balance,
                        Term = currentMortgage.Term,
                        IsUpForRenewal = false,
                        InterestRate = currentMortgage.InterestRate - 4,
                        Type = MortgageType.Fixed2Years,
                        Description = string.Format("Fixed 2 years {0}%", currentMortgage.InterestRate - 2),
                    },
                    new Mortgage
                    {
                        Balance = currentMortgage.Balance,
                        Term = currentMortgage.Term - 4,
                        IsUpForRenewal = false,
                        InterestRate = currentMortgage.InterestRate - 2,
                        Type = MortgageType.Fixed5Years,
                        Description = string.Format("Fixed 5 years {0}%", currentMortgage.InterestRate - 2),
                    },
                    new Mortgage
                    {
                        Balance = currentMortgage.Balance,
                        Term = currentMortgage.Term - 10,
                        IsUpForRenewal = false,
                        InterestRate = currentMortgage.InterestRate - 4,
                        Type = MortgageType.Fixed2Years,
                        Description = string.Format("Fixed 2 years {0}%", currentMortgage.InterestRate - 4),
                    },
                };
            }
            else
            {
                return null;
            }
        }
    }
}
