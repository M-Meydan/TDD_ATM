using ATM.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace ATM.UnitTests.FeatureTests
{
    [TestFixture(Category = "Domain: Account")]
    public class AccountTests
    {
        Account _account;
      
        #region Setup
        [SetUp]
        public void Setup()
        {
            _account = new Account();
        }

        // [TearDown]
        public void Teardown() { }

        #endregion

        [TestCase(10,true)]
        [TestCase(50,true)]
        [TestCase(100,true)]
        [TestCase(200, true)]
        [TestCase(500, false)]
        [TestCase(600, false)]
        public void FundsLow_returns_true_when_balance_lower_than_lowfundsamount(decimal balance, bool expectedResult)
        {           
            _account.Deposit(balance);

            _account.FundsLow().Should().Be(expectedResult);
        }

        [TestCase(100, false)]
        [TestCase(200, false)]
        [TestCase(1000, false)]
        [TestCase(2000, false)]
        [TestCase(3000, false)]
        [TestCase(3400, false)]
        [TestCase(3550, true)]
        [TestCase(3600, true)]
        [TestCase(3700, true)]
        [TestCase(3800, true)]
        [TestCase(3900, true)]
        [TestCase(3950, true)]
        [TestCase(4000, true)]
        public void PayInLimitApproaching_returns_true_when_payinlimit_left_lower_than_lowfundsamount(decimal transferAmount, bool expectedResult)
        {
            _account.PayInLimitApproaching(transferAmount).Should().Be(expectedResult);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Withdraw_should_throw_InvalidAmountException_when_transferamount_is_zero_or_less(decimal withdrawalAmount)
        {
            FluentActions.Invoking(() => _account.Withdraw(withdrawalAmount))
                .Should().Throw<InvalidAmountException>();
        }

        [Test]
        public void Withdraw_should_throw_InsufficientFundsException_when_balance_is_less_than_zero()
        {
            var withdrawalAmount = 50;
            _account.Deposit(10);

            FluentActions.Invoking(() => _account.Withdraw(withdrawalAmount))
                .Should().Throw<InsufficientFundsException>();
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Deposit_should_throw_InvalidAmountException_when_transferamount_is_zero_or_less(decimal transferAmount)
        {
            FluentActions.Invoking(() => _account.Deposit(transferAmount))
                .Should().Throw<InvalidAmountException>();
        }

        [Test]
        public void Deposit_should_throw_PayInLimitReachedException_when_transferamount_greater_than_payinlimit()
        {
            var transferAmount = 4010;
            _account.Deposit(1);

            FluentActions.Invoking(() => _account.Deposit(transferAmount))
                .Should().Throw<PayInLimitReachedException>();
        }

    }
}