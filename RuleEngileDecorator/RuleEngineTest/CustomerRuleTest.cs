using System;
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
    }
}
