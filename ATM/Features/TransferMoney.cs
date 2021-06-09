using ATM.DataAccess;
using ATM.Domain.Services;
using System;

namespace ATM.Features
{
    public class TransferMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid senderAccountId, Guid receiverAccountId, decimal amount)
        {
            var senderAccount = _accountRepository.GetAccountById(senderAccountId);
            var receiverAccount = _accountRepository.GetAccountById(receiverAccountId);

            senderAccount.Withdraw(amount);
            receiverAccount.Deposit(amount);

            _accountRepository.Update(senderAccount);
            _accountRepository.Update(receiverAccount);

            //When updates successful then send the notifications
            if (senderAccount.FundsLow())
                _notificationService.NotifyFundsLow(senderAccount.User.Email);

            if (receiverAccount.PayInLimitApproaching(amount))
                _notificationService.NotifyApproachingPayInLimit(receiverAccount.User.Email);
        }

    }
}
