using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.ServiceBus.Messaging;
using SendGrid;

namespace BugTrackingSystem.BusQueueListener
{
    class Program
    {
        private const string ConnectionString = "Endpoint=sb://blinov.servicebus.windows.net/;SharedAccessKeyName=ConsumerAccessKey;SharedAccessKey=7y8L/m9iPfc9V0Xrn18w5pAcTmMBef2GYRCVNTgCxLw=";
        private const string QueueName = "asignar";

        static void Main(string[] args)
        {
            Listen();
        }

        private static void Listen()
        {
            var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);

            try
            {
                //var shutdownFilePath = Environment.GetEnvironmentVariable("WEBJOBS_SHUTDOWN_FILE");

                //if (string.IsNullOrEmpty(shutdownFilePath))
                //{
                //    throw new InvalidOperationException("WEBJOBS_SHUTDOWN_FILE variable has empty value");
                //}

                //while (!File.Exists(shutdownFilePath))
                while (true)
                {
                    var message = client.Receive();

                    if (message == null)
                        continue;

                    var queueMessage = (string) message.Properties["MessageInfo"];
                    var fromAddress = new MailAddress((string)message.Properties["UserFromEmail"],
                        (string)message.Properties["UserFromName"]);
                    var toAddress = new MailAddress((string)message.Properties["UserToEmail"],
                        (string)message.Properties["UserToName"]);

                    var sendGridMessage = new SendGridMessage
                    {
                        To = new[] {toAddress},
                        From = fromAddress,
                        Subject = (string) message.Properties["Subject"],
                        Html = "Email content here"
                    };

                    sendGridMessage.EnableTemplateEngine("cc59ef33-2451-4505-afa6-b1a7e362783a");
                    sendGridMessage.AddSubstitution("%userTo%", new List<string> { (string)message.Properties["UserToName"] });
                    sendGridMessage.AddSubstitution("%body%", new List<string>() {queueMessage});
                    sendGridMessage.AddSubstitution("%userFrom%", new List<string>{ (string)message.Properties["UserFromName"] });

                    var sendGridNetworkCredential = new NetworkCredential("azure_4c1f2e65473355288c560030a6e88b2a@azure.com", "KPgZDZiM7c8hXHz");
                    var transportWeb = new Web(sendGridNetworkCredential);

                    transportWeb.DeliverAsync(sendGridMessage).Wait();
                    message.Complete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
