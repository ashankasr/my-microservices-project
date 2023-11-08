using System;
using Play.Common.Entities;

namespace Play.Inventory.Service.Entities
{
    public class CatalogItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}