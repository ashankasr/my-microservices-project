using System;

namespace Play.Catalog.Service.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        // public DateTimeOffset CreatedDate { get; set; }
        // public DateTimeOffset UpdatedDate { get; set; }
    }
}