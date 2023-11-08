using System;
using Play.Common.Entities;

namespace Play.Catalog.Service.Entities
{
    public class Item : BaseAuditableEntity
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}