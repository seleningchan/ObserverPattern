using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;

namespace EventDemo
{
    public class InProcessDomainEventPublisher : IDomainEventPublisher
    {
        private readonly IServiceProvider serviceProvider;

        public InProcessDomainEventPublisher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            var handlers = serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}
