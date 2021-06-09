using ATM.DataAccess;
using ATM.Domain.Exceptions;
using ATM.Domain.Services;
using ATM.Features;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ATM.UnitTests.FeatureTests
{
    [TestFixture(Category = "Features: WithdrawMoney")]
    public class WithdrawMoneyTests
    {
        WithdrawMoney _withdrawMoney;
        IAccountRepository _accountRepository;
        INotificationService _notificationService;

        #region Setup
        [SetUp]
        public void Setup()
        {
            _accountRepository= A.Fake<IAccountRepository>();
            _notificationService = A.Fake<INotificationService>();
            _withdrawMoney = new WithdrawMoney(_accountRepository, _notificationService);
        }

        // [TearDown]
        public void Teardown() { }

        #endregion

        [Test]
        public void Withdrawal_should_succeed_when_account_has_sufficent_balance()
        {
            var initialBalance = 1000;
            var withdrawalAmount = 50;

            var account = A.Fake<Account>();
            account.Deposit(initialBalance);

            A.CallTo(() => _accountRepository.GetAccountById(A<Guid>._)).Returns(account);

            _withdrawMoney.Execute(Guid.NewGuid(), withdrawalAmount);

            A.CallTo(() => _accountRepository.Update(A<Account>._)).MustHaveHappened();
            account.Balance.Should().BeLessThan(initialBalance);
        }

        [Test]
        public void Withdrawal_should_fail_when_account_has_insufficent_balance()
        {
            var initialBalance = 10;
            var withdrawalAmount = 50;

            var account = A.Fake<Account>();
            account.Deposit(initialBalance);

            FluentActions.Invoking(() => _withdrawMoney.Execute(Guid.NewGuid(), withdrawalAmount))
                .Should().Throw<InsufficientFundsException>();
        }

        [Test]
        public void Notification_should_be_sent_when_account_balance_is_low()
        {
            var initialBalance = 50;
            var withdrawalAmount = 30;

            var account = A.Fake<Account>();
            account.Deposit(initialBalance);
            account.User = new User() { Email = "test@test.com"};

            A.CallTo(() => _accountRepository.GetAccountById(A<Guid>._)).Returns(account);

            _withdrawMoney.Execute(Guid.NewGuid(), withdrawalAmount);

            A.CallTo(() => _accountRepository.Update(A<Account>._)).MustHaveHappened();
            A.CallTo(() => _notificationService.NotifyFundsLow(A<string>._)).MustHaveHappened();
        }
    }
}