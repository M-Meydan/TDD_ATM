using System;

namespace ATM.Domain.Exceptions
{
    public class InsufficientFundsException : ApplicationException
    {
        public InsufficientFundsException() : base("Insufficient funds to make transfer") { }
        public InsufficientFundsException(string message) : base(message) { }
    }
}