using MassTransit;
using System;

namespace Payments
{
    class Program
    {
        // [Obsolete]
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "Payments", ep =>
                {
                    ep.Consumer( () => new OrderRequestedConsumer() );
                });
            });

            bus.Start();

            Console.WriteLine("Welcome to Payments");
            Console.WriteLine("Press Q key to exit");

            while (Console.ReadKey(true).Key != ConsoleKey.Q) ;

            bus.Stop();
        }
    }
}