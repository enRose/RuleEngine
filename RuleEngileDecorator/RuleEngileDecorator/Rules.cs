using System;
using System.Threading.Tasks;

namespace RuleEngileDecorator
{
    public class CustomerRuleContext
    {
        public string Id { get; set; }
        public DateTime Dob { get; set; }
        public CustomerAddress Address { get; set; }
    }

    public class CustomerAddress
    {
        public string Address { get; set; }
        public DateTime MovedInDate { get; set; }
    }

    [Rule(Category = "Customer", priority = 0)]
    public class MustBe18
    {
        [Run]
        public bool IsValid(CustomerRuleContext ctx) =>
            ctx.Dob.Date.AddYears(18) <= DateTime.Today.Date;
    }

    [Rule(Category = "Customer", priority = 1)]
    public class MustLiveInCurrentAddressAtLeast1Year
    {
        [Run]
        public bool IsValid(CustomerRuleContext ctx) =>
            ctx.Address?.MovedInDate.Date.AddYears(1) <= DateTime.Today.Date;
    }

    [Rule(Category = "Customer", priority = 2)]
    public class MustBe18AndLiveInCurrentAddressAtLeast1Year
    {
        [Run]
        public bool IsOver18(CustomerRuleContext ctx) =>
            ctx.Dob.Date.AddYears(18) <= DateTime.Today.Date;

        [Run]
        public bool LiveInCurrentAddressLeast1Year(CustomerRuleContext ctx) =>
            ctx.Address?.MovedInDate.Date.AddYears(1) <= DateTime.Today.Date;
    }

    [Rule(Category = "Customer", priority = 3)]
    public class MustHaveActiveMembership
    {
        [RunAsync]
        public async Task<bool> HasActiveMembership(CustomerRuleContext ctx)
        {
            // Simulate fetching from backend.
            await Task.Delay(50);

            return ctx.Id != "No active membership";
        }
    }

    [Rule(Category = "Customer", priority = 4)]
    public class TwoFactorAuthRegistered
    {
        [RunAsync]
        public async Task<bool> IsTwoFactorAuthRegistered(CustomerRuleContext ctx)
        {
            // Simulate fetching from backend.
            await Task.Delay(30);

            return ctx.Id != "No two factor auth";
        }
    }

    [Rule(Category = "Customer", priority = 5)]
    public class TwoFactorAuthRegisteredAndActiveMembership
    {
        [RunAsync]
        public async Task<bool> IsTwoFactorAuthRegistered(CustomerRuleContext ctx)
        {
            // Simulate fetching from backend.
            await Task.Delay(30);

            return ctx.Id != "No two factor auth";
        }

        [RunAsync]
        public async Task<bool> HasActiveMembership(CustomerRuleContext ctx)
        {
            // Simulate fetching from backend.
            await Task.Delay(50);

            return ctx.Id != "No active membership";
        }
    }
}
