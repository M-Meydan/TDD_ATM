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
    [TestFixture(Category = "Features: TransferMoney")]
    public class TransferMoneyTests
    {
        TransferMoney _transferMoney;
        IAccountRepository _accountRepository;
        INotificationService _notificationService;

        #region Setup
        [SetUp]
        public void Setup()
        {
            _accountRepository= A.Fake<IAccountRepository>();
            _notificationService = A.Fake<INotificationService>();
            _transferMoney = new TransferMoney(_accountRepository, _notificationService);
        }

        // [TearDown]
        public void Teardown() { }

        #endregion

        [Test]
        public void Transfer_should_succeed_when_sender_account_has_sufficent_balance()
        {
            var initialBalance = 1000;
            var transferAmount = 50;

            var senderAccount = new Account { Id = Guid.NewGuid() };
            senderAccount.Deposit(initialBalance);

            var receiverAccount = new Account { Id = Guid.NewGuid() };
            receiverAccount.Deposit(initialBalance);

            A.CallTo(() => _accountRepository.GetAccountById(senderAccount.Id)).Returns(senderAccount);
            A.CallTo(() => _accountRepository.GetAccountById(receiverAccount.Id)).Returns(receiverAccount);

            _transferMoney.Execute(senderAccount.Id, receiverAccount.Id, transferAmount);

            A.CallTo(() => _accountRepository.Update(senderAccount)).MustHaveHappened();
            A.CallTo(() => _accountRepository.Update(receiverAccount)).MustHaveHappened();
            senderAccount.Balance.Should().BeLessThan(initialBalance);
            receiverAccount.Balance.Should().BeGreaterThan(initialBalance);
        }

        [Test]
        public void Transfer_should_fail_when_sender_account_has_insufficent_balance()
        {
            var initialBalance = 10;
            var transferAmount = 50;

            var senderAccount = new Account { Id = Guid.NewGuid() };
            senderAccount.Deposit(initialBalance);

            var receiverAccount = new Account { Id = Guid.NewGuid() };
            receiverAccount.Deposit(initialBalance);

            A.CallTo(() => _accountRepository.GetAccountById(senderAccount.Id)).Returns(senderAccount);
            A.CallTo(() => _accountRepository.GetAccountById(receiverAccount.Id)).Returns(receiverAccount);

            FluentActions.Invoking(() => _transferMoney.Execute(senderAccount.Id, receiverAccount.Id, transferAmount))
                .Should().Throw<InsufficientFundsException>();
        }

        [Test]
        public void Notification_should_be_sent_when_sender_account_balance_is_low()
        {
            var initialBalance = 50;
            var transferAmount = 30;
           
            var senderAccount = new Account { Id = Guid.NewGuid() };
            senderAccount.Deposit(initialBalance);
            senderAccount.User = new User() { Email = "test1@test.com" }; ;

            var receiverAccount = new Account { Id = Guid.NewGuid() };
            receiverAccount.Deposit(initialBalance);
            receiverAccount.User = new User() { Email = "test2@test.com" }; ;

            A.CallTo(() => _accountRepository.GetAccountById(senderAccount.Id)).Returns(senderAccount);
            A.CallTo(() => _accountRepository.GetAccountById(receiverAccount.Id)).Returns(receiverAccount);

            _transferMoney.Execute(senderAccount.Id, receiverAccount.Id, transferAmount);

            A.CallTo(() => _accountRepository.Update(senderAccount)).MustHaveHappened();
            A.CallTo(() => _accountRepository.Update(receiverAccount)).MustHaveHappened();
            A.CallTo(() => _notificationService.NotifyFundsLow(senderAccount.User.Email)).MustHaveHappened();
        }

        [Test]
        public void Notification_should_be_sent_when_receiver_payin_limit_approached()
        {
            var initialBalance = 3500;
            var transferAmount = 30;
            
            var senderAccount = new Account { Id = Guid.NewGuid() };
            senderAccount.Deposit(initialBalance);
            senderAccount.User = new User() { Email = "test1@test.com" }; 

            var receiverAccount = new Account { Id = Guid.NewGuid() };
            receiverAccount.Deposit(initialBalance);
            receiverAccount.User = new User() { Email = "test2@test.com" }; ;

            A.CallTo(() => _accountRepository.GetAccountById(senderAccount.Id)).Returns(senderAccount);
            A.CallTo(() => _accountRepository.GetAccountById(receiverAccount.Id)).Returns(receiverAccount);

            _transferMoney.Execute(senderAccount.Id, receiverAccount.Id, transferAmount);

            A.CallTo(() => _accountRepository.Update(senderAccount)).MustHaveHappened();
            A.CallTo(() => _accountRepository.Update(receiverAccount)).MustHaveHappened();
            A.CallTo(() => _notificationService.NotifyApproachingPayInLimit(receiverAccount.User.Email)).MustHaveHappened();
        }
    }
}