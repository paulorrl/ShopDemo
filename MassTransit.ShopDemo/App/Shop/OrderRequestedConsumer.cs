using MassTransit;
using Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop
{
    public class OrderRequestedConsumer : IConsumer<Fault<IOrderRequested>>
    {
        public Task Consume(ConsumeContext<Fault<IOrderRequested>> context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The following order was not processed:");

            foreach (var productGroup in context.Message.Message.Products.GroupBy(x => x.Name))
            {
                Console.WriteLine($"{productGroup.Key} x{productGroup.Count()}");
            }

            Console.WriteLine(string.Empty);
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}