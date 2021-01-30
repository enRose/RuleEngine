using System;
using System.Threading.Tasks;
using RuleEngileDecorator;
using Xunit;

namespace RuleEngineTest
{
    public class CustomerRuleTest
    {
        [Fact]
        public void ShouldFailOnAgeRule()
        {
            var ctx = new CustomerRuleContext()
            {
                Dob = DateTime.Today.Date.AddYears(-16),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddYears(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = ruleEngine.IsValid();

            Assert.False(result, "Customer age 16");
        }

        [Fact]
        public void ShouldPassAgeRule()
        {
            var ctx = new CustomerRuleContext()
            {
                Dob = DateTime.Today.Date.AddYears(-20),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddYears(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = ruleEngine.IsValid();

            Assert.True(result, "Customer age 20");
        }

        [Fact]
        public void ShouldFailAddressRule()
        {
            var ctx = new CustomerRuleContext()
            {
                Dob = DateTime.Today.Date.AddYears(-20),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddMonths(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = ruleEngine.IsValid();

            Assert.False(result, "Not in current address for at least one year");
        }

        [Fact]
        public void ShouldPassAgeAndAddressRule()
        {
            var ctx = new CustomerRuleContext()
            {
                Dob = DateTime.Today.Date.AddYears(-20),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddYears(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = ruleEngine.IsValid();

            Assert.True(result, "Customer age 20, at current address for 2 years");
        }

        [Fact]
        public async Task ShouldFailWhenNoActiveMembership()
        {
            var ctx = new CustomerRuleContext()
            {
                Id = "No active membership",
                Dob = DateTime.Today.Date.AddYears(-20),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddYears(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = await ruleEngine.IsValidAsync();

            Assert.False(result, "No active membership");
        }

        [Fact]
        public async Task ShouldFailWhenNot2FactorRegistered()
        {
            var ctx = new CustomerRuleContext()
            {
                Id = "No two factor auth",
                Dob = DateTime.Today.Date.AddYears(-20),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddYears(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = await ruleEngine.IsValidAsync();

            Assert.False(result, "Not two factor auth registered");
        }

        [Fact]
        public async Task ShouldPassWhen2FactorRegisteredAndHasActiveMembership()
        {
            var ctx = new CustomerRuleContext()
            {
                Id = "1",
                Dob = DateTime.Today.Date.AddYears(-20),
                Address = new CustomerAddress()
                {
                    Address = "16 Adams Street, Auckland, NZ",
                    MovedInDate = DateTime.Today.Date.AddYears(-2)
                }
            };

            var ruleEngine = new RuleEngine("Customer", ctx);

            var result = await ruleEngine.IsValidAsync();

            Assert.True(result, "Two factor auth registered, has active membership");
        }
    }
}
