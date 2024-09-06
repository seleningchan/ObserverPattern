using System;
using System.Collections.Generic;
using System.Text;

namespace EventDemo
{
    public class CustomerEmailChangedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public string OldEmail { get; }
        public Customer Customer { get; }

        public CustomerEmailChangedEvent(Customer customer, string oldEmail)
        {
            Customer = customer;
            OldEmail = oldEmail;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
