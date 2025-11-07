using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UAP.SharedKernel.Entity
{
    /// <summary>
    /// Aggregate root marker interface
    /// Represents the root entity of an aggregate that maintains consistency boundaries
    /// </summary>
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        protected AggregateRoot(TId id) : base(id) { }
        protected AggregateRoot() { }
    }
}
