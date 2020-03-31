using MassTransit;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop
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

                sbc.ReceiveEndpoint(host, "Shop", ep =>
                {
                    ep.Consumer(() => new OrderRequestedConsumer());
                });
            });

            bus.Start();

            Console.WriteLine("Welcome to the Shop");
            Console.WriteLine("Press Q key to exit");
            Console.WriteLine("Press [0..9] key to order some products");
            Console.WriteLine(string.Join(Environment.NewLine, Products.Select((x, i) => $"[{i}]: {x.name} @ {x.price:C}") ));

            var products = new List<(string name, decimal price)>();
            Console.WriteLine(Environment.NewLine + "Order:" );

            for (;;)
            {
                var consoleKeyInfo = Console.ReadKey(true);

                if (consoleKeyInfo.Key == ConsoleKey.Q)
                {
                    break;
                }

                if (char.IsNumber(consoleKeyInfo.KeyChar))
                {
                    // Hack: I don't care about ½ etc...

                    var product = Products[ (int) char.GetNumericValue(consoleKeyInfo.KeyChar) ];
                    products.Add(product);
                    Console.WriteLine($"Added {product.name}");
                }

                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    bus.Publish<IOrderRequested>(new
                    {
                        Products = products.Select(x => new { Name = x.name, Price = x.price }).ToList()
                    });

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Submitted Order" + Environment.NewLine);
                    Console.ResetColor();
                    products.Clear();
                }
            }

            bus.Stop();
        }

        private static readonly IReadOnlyList<(string name, decimal price)> Products = new List<(string, decimal)>
        {
            ("Bread", 1.20m),
            ("Milk", 0.50m),
            ("Rice", 1m),
            ("Pasta", 0.9m),
            ("Cereals", 1.6m),
            ("Chocolate", 2m),
            ("Noodles", 1m),
            ("Pie", 1m),
            ("Sandwich", 2m),
            ("Biscoit", 0.5m)
        };
    }
}