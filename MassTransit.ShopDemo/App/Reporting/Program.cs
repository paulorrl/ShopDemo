﻿using MassTransit;
using System;
using System.Linq;

namespace Reporting
{
    class Program
    {
        // [Obsolete]
        static void Main(string[] args)
        {
            var reportStore = new ReportStore();
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "Reporting", ep =>
                {
                    ep.Consumer( () => new OrderRequestedConsumer(reportStore));
                    ep.Consumer( () => new OrderAcceptedConsumer(reportStore));
                });
            });

            bus.Start();

            Console.WriteLine("Welcome to Reports");
            Console.WriteLine("Press Q key to exit");
            Console.WriteLine("Press R key to show report");

            for (;;)
            {
                var consoleKeyInfo = Console.ReadKey(true);

                if (consoleKeyInfo.Key == ConsoleKey.Q)
                    break;

                if (consoleKeyInfo.Key == ConsoleKey.R)
                {
                    Console.WriteLine(Environment.NewLine + "-- Product Sales --");

                    var message = string.Join(Environment.NewLine, reportStore.ProductSales.Select(x => $" {x.Key}: {x.Value:C}" ));
                    Console.WriteLine(message + Environment.NewLine);

                    Console.WriteLine("-- Totals --");
                    Console.WriteLine($"Total orders requested: {reportStore.TotalOrdersRequested:C}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Total orders accepted: {reportStore.TotalOrdersAccepted:C}");

                    if (reportStore.Unsold() < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Total orders unsold: {(reportStore.Unsold() * (-1) ):C}");
                    }

                    Console.ResetColor();
                }
            }

            bus.Stop();
        }
    }
}