using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Messaging;
using System.Threading;

namespace SimpleSender
{
    class Program
    {
        static string eventHubName = "test";
        static string connectionString = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl-C to stop the sender process");
            Console.WriteLine("Press Enter to start now");
            Console.ReadLine();
            SendingRandomMessages();
        }


        static void SendingRandomMessages()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
            int count = 10;
            while (count>0)
            {
                try
                {
                    //var message = Guid.NewGuid().ToString();
                    //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                    //eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(message)));

                    Console.WriteLine(count);
                    byte[] payloadBytes = Encoding.UTF8.GetBytes("Test message from Simple Sender "+ count);
                    EventData sendEvent = new EventData(payloadBytes);
                    eventHubClient.Send(sendEvent);
                    count--;
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(200);
            }
        }

    }
}
