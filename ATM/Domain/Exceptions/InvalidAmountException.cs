using System;

namespace ATM.Domain.Exceptions
{
    public class InvalidAmountException : ApplicationException
    {
        public InvalidAmountException() : base("Invalid amount!") { }
        public InvalidAmountException(string message) : base(message) { }
    }
}