using System;

namespace RuleEngileDecorator
{
    public class CustomerAddress
    {
        public string Address { get; set; }
        public DateTime MovedInDate { get; set; }
    }

    public class CustomerRuleContext
    {
        public DateTime Dob { get; set; }
        public CustomerAddress Address { get; set; }
    }

    [Rule(Category = "Customer", priority = 1)]
    public class MustBe18
    {
        [Run]
        public bool IsValid(CustomerRuleContext ctx) =>
            ctx.Dob.Date.AddYears(18) <= DateTime.Today.Date;
    }

    [Rule(Category = "Customer", priority = 2)]
    public class MustLiveInCurrentAddressAtLeast1Year
    {
        [Run]
        public bool IsValid(CustomerRuleContext ctx) =>
            ctx.Address?.MovedInDate.Date.AddYears(1) <= DateTime.Today.Date;
    }
}
