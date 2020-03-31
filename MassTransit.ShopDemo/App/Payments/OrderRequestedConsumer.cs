using MassTransit;
using Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Payments
{
    public class OrderRequestedConsumer : IConsumer<IOrderRequested>
    {
        public async Task Consume(ConsumeContext<IOrderRequested> context)
        {
            if (TakePayment(context.Message))
            {
                await context.Publish<IOrderAccepted>(new { context.Message.Products });

                Console.WriteLine("Paid Order:");
                
                foreach (var product in context.Message.Products)
                {
                    Console.WriteLine($" {product.Name} - {product.Price:C}");
                }

                Console.WriteLine($"Amount paid: {context.Message.Products.Sum(x => x.Price):C} {Environment.NewLine}");
            }
            else
            {
                throw new Exception("Payment failed");
            }
        }

        private bool TakePayment(IOrderRequested contextMessage)
        {
            // 1 in 10 payments fail
            var paymentFailed = Random.Next(1, 10) == 1;

            if (paymentFailed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Payment Failed" + Environment.NewLine);
            }
            if (!paymentFailed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Payment Successful" + Environment.NewLine);
            }

            Console.ResetColor();
            return !paymentFailed;
        }

        private static readonly Random Random = new Random();
    }
}