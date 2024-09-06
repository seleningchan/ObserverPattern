using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace EventDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDomainEventPublisher, InProcessDomainEventPublisher>();
            serviceCollection.AddSingleton<IDomainEventHandler<CustomerEmailChangedEvent>, CustomerEmailChangedEventHandler>();
            var customer = new Customer
            {
                Name = "NewCustomer"
            };
            var oldEmail = "old@email.com";
            using IServiceScope scope = serviceCollection.BuildServiceProvider().CreateScope();
            var provider = scope.ServiceProvider;
            await provider.GetRequiredService<IDomainEventPublisher>()
                .PublishAsync(new CustomerEmailChangedEvent(customer, oldEmail));

            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }
    }
}
