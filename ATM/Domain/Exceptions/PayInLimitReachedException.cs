using System;

namespace ATM.Domain.Exceptions
{
    public class PayInLimitReachedException : ApplicationException
    {
        public PayInLimitReachedException() : base("Account pay in limit reached") { }
        public PayInLimitReachedException(string message) : base(message) { }
    }
}