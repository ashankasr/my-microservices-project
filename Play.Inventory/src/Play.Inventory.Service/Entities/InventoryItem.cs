using System;
using Play.Common.Entities;

namespace Play.Inventory.Service.Entities
{
    public class InventoryItem : BaseAuditableEntity
    {
        public Guid UserId { get; set; }

        public Guid CatalogItemId { get; set; }
        public int Quantity { get; set; }

        public DateTimeOffset AcquiredDate { get; set; }
    }
}