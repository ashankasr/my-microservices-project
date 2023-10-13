using System;

namespace Play.Common.Entities
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}