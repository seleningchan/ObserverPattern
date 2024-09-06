using System;
using System.Collections.Generic;
using System.Text;

namespace EventDemo
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
