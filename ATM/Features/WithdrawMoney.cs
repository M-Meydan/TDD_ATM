using ATM.DataAccess;
using ATM.Domain.Services;
using System;

namespace ATM.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid accountId, decimal amount)
        {
            var account = _accountRepository.GetAccountById(accountId);

            account.Withdraw(amount);

            _accountRepository.Update(account);

            //When updates successful then send the notifications
            if (account.FundsLow())
                _notificationService.NotifyFundsLow(account.User.Email);
       }
    }
}
