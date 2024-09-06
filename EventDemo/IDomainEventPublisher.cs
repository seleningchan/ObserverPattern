using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventDemo
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }
}
