using ATM.Domain.Exceptions;
using System;

namespace ATM
{

    public class Account
    {
        const decimal PayInLimit = 4000m;
        const decimal LowFundsAmount = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public bool FundsLow() => Balance < LowFundsAmount;

        public bool PayInLimitApproaching(decimal transferAmount) => PayInLimit - (PaidIn + transferAmount) < LowFundsAmount;

        public void Withdraw(decimal transferAmount)
        {
            if (AmountInvalid(transferAmount))  throw new InvalidAmountException();

            if (FundsInsufficient(transferAmount))  throw new InsufficientFundsException();

            Withdrawn -= transferAmount;
            Balance -= transferAmount;
        }

        public void Deposit(decimal transferAmount)
        {
            if (AmountInvalid(transferAmount))  throw new InvalidAmountException();

            if (PayInLimitReached(transferAmount))  throw new PayInLimitReachedException();

            PaidIn += transferAmount;
            Balance += transferAmount;
        }

        bool AmountInvalid(decimal transferAmount) => transferAmount <= 0m;

        bool FundsInsufficient(decimal transferAmount) => (Balance - transferAmount) < 0m;

        bool PayInLimitReached(decimal transferAmount) => (PaidIn + transferAmount) > PayInLimit;

    }
}
