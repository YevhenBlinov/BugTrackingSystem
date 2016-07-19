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
        private const string QueueName = "asignarbts";
        private const string MyEmailAdress = "asignartester@gmail.com";
        private const string Password = "zxcvbasdfg";

        static void Main(string[] args)
        {
            Listen();
        }

        private static void Listen()
        {
            var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);

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

                try
                {
                    //var fromAddress = new MailAddress((string)message.Properties["UserFromEmail"],
                    //    (string)message.Properties["UserFromName"]);
                    //var toAddress = new MailAddress((string)message.Properties["UserToEmail"],
                    //    (string)message.Properties["UserToName"]);

                    //var sendGridMessage = new SendGridMessage
                    //{
                    //    To = new[] {toAddress},
                    //    From = fromAddress,
                    //    Subject = (string) message.Properties["Subject"],
                    //    Html = "Email content here",
                    //    Text = "Email content here"
                    //};

                    //var messageType = (string)message.Properties["Type"];

                    //switch (messageType)
                    //{
                    //    case "ResetPassword":
                    //    {
                    //        var resetPasswordUrl = (string) message.Properties["ResetPasswordUrl"];
                    //        sendGridMessage.EnableTemplateEngine("cc59ef33-2451-4505-afa6-b1a7e362783a");
                    //        sendGridMessage.AddSubstitution("%userTo%",
                    //            new List<string> {(string) message.Properties["UserToName"]});
                    //        sendGridMessage.AddSubstitution("%body%", new List<string>() {resetPasswordUrl});
                    //        sendGridMessage.AddSubstitution("%userFrom%",
                    //            new List<string> {(string) message.Properties["UserFromName"]});
                    //        break;
                    //    }
                    //    case "AssigneeChanged":
                    //    {
                    //        var bugTitle = (string) message.Properties["BugTitle"];
                    //        var projectName = (string) message.Properties["ProjectName"];
                    //        sendGridMessage.EnableTemplateEngine("0f1d9a61-2f3f-4866-a340-966ac567a27a");
                    //        sendGridMessage.AddSubstitution("%userTo%",
                    //            new List<string> {(string) message.Properties["UserToName"]});
                    //        sendGridMessage.AddSubstitution("%body%", new List<string>() {bugTitle});
                    //        sendGridMessage.AddSubstitution("%project%", new List<string>() {projectName});
                    //        sendGridMessage.AddSubstitution("%userFrom%",
                    //            new List<string> {(string) message.Properties["UserFromName"]});
                    //        break;
                    //    }
                    //    case "BugStateChanged":
                    //    {
                    //        var bugTitle = (string) message.Properties["BugTitle"];
                    //        var projectName = (string) message.Properties["ProjectName"];
                    //        sendGridMessage.EnableTemplateEngine("8a8c620c-65eb-4eba-8084-796fd5625d4a");
                    //        sendGridMessage.AddSubstitution("%userTo%",
                    //            new List<string> {(string) message.Properties["UserToName"]});
                    //        sendGridMessage.AddSubstitution("%body%", new List<string>() {bugTitle});
                    //        sendGridMessage.AddSubstitution("%project%", new List<string>() {projectName});
                    //        sendGridMessage.AddSubstitution("%userFrom%",
                    //            new List<string> {(string) message.Properties["UserFromName"]});
                    //        break;
                    //    }
                    //    default:
                    //    {
                    //        message.Abandon();
                    //        continue;
                    //    }
                    //}

                    //var sendGridNetworkCredential = new NetworkCredential("azure_4c1f2e65473355288c560030a6e88b2a@azure.com", "KPgZDZiM7c8hXHz");
                    //var transportWeb = new Web(sendGridNetworkCredential);

                    //transportWeb.DeliverAsync(sendGridMessage).Wait();
                    var fromAddress = new MailAddress(MyEmailAdress, "Asignar support");
                    var toAddress = new MailAddress((string)message.Properties["UserToEmail"],
                        (string)message.Properties["UserToName"]);
                    var body = String.Empty;
                    var messageType = (string)message.Properties["Type"];

                    switch (messageType)
                    {
                        case "ResetPassword":
                            {
                                body =
                                    String.Format(
                                        "Hello, {0}!\r\n" +
                                        "You recently requested to reset your password for your Asignar account.\r\n"+
                                        "Please, follow the link to reset your password: {1}.\r\n" +
                                        "If you did not request a password reset, please ignore this email or reply to let us know.\r\n" +
                                        "Best regards, Asignar support.",
                                        (string)message.Properties["UserToName"],
                                        (string)message.Properties["ResetPasswordUrl"]);
                                break;
                            }
                        case "AssigneeChanged":
                        {
                            body =
                                String.Format(
                                    "Hello, {0}!\r\n" +
                                    "You've been assigned to bug with title {1} of project {2}.\r\n" +
                                    "Please visit our website http://asignar.azurewebsites.net for details.\r\n" +
                                    "Best regards, Asignar support.",
                                    (string)message.Properties["UserToName"], (string)message.Properties["BugTitle"],
                                    (string)message.Properties["ProjectName"]);
                                break;
                            }
                        default:
                            {
                                message.Abandon();
                                continue;
                            }
                    }

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, Password)
                    };

                    using (var messageToSend = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = (string)message.Properties["Subject"],
                        Body = body
                    })
                    {
                        smtp.Send(messageToSend);
                    }

                    message.Complete();
                }
                catch (Exception)
                {
                    message.Abandon();
                }
            }
        }
    }
}
