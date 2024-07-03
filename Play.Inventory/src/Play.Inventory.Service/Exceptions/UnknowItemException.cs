using System;
using System.Runtime.Serialization;

namespace Play.Inventory.Service.Exceptions
{
    [Serializable]
    public class UnknowItemException : Exception
    {

        public UnknowItemException(Guid itemId)
        : base($"Unknown item ''{itemId}''.") // "Unknown item '{itemId}'.
        {
            this.ItemId = itemId;
        }

        public Guid ItemId { get; }
    }
}