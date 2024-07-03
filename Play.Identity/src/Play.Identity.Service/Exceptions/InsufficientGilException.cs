using System;
using System.Runtime.Serialization;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class InsufficientGilException : Exception
    {
        public Guid UserId { get; }
        public decimal GilToDebit { get; }

        public InsufficientGilException(Guid userId, decimal gilToDebit) : base($"Insufficient gil to debit {gilToDebit} from user {userId}")
        {
            this.UserId = userId;
            this.GilToDebit = gilToDebit;
        }
    }
}