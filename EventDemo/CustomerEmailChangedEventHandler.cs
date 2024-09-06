using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDemo
{
    public class CustomerEmailChangedEventHandler : IDomainEventHandler<CustomerEmailChangedEvent>
    {
        public Task HandleAsync(CustomerEmailChangedEvent @event)
        {
            Console.WriteLine($"Cutomer {@event.Customer.Name} email changed from {@event.OldEmail} at {@event.OccurredOn}");
            return Task.CompletedTask;
        }
    }
}
