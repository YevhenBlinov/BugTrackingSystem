using Microsoft.ServiceBus.Messaging;

namespace BugTrackingSystem.AzureService
{
    public static class BusQueueService
    {
        private const string ConnectionString =
            "Endpoint=sb://blinov.servicebus.windows.net/;SharedAccessKeyName=ProducerAccessKey;SharedAccessKey=0GwrLQ98xtYfZwDtT66pOqCvz6hTCjM1XjHnZ9J72HA=";
        private const string QueueName = "asignar";

        public static void AddMessageToQueue(string userToName, string userToEmail, string userFromName, string userFromEmail, string messageInfo)
        {
            var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);
            var message = new BrokeredMessage();
            message.Properties.Add("UserToName", userToName);
            message.Properties.Add("UserFromName", userFromName);
            message.Properties.Add("UserToEmail", userToEmail);
            message.Properties.Add("UserFromEmail", userFromEmail);
            message.Properties.Add("MessageInfo", messageInfo);
            message.Properties.Add("Subject", "Reset password request");
            client.Send(message);
        }
    }
}
