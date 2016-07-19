using Microsoft.ServiceBus.Messaging;

namespace BugTrackingSystem.AzureService
{
    public static class BusQueueService
    {
        private const string ConnectionString =
            "Endpoint=sb://blinov.servicebus.windows.net/;SharedAccessKeyName=ProducerAccessKey;SharedAccessKey=0GwrLQ98xtYfZwDtT66pOqCvz6hTCjM1XjHnZ9J72HA=";
        private const string QueueName = "asignarbts";

        //public static void AddResetPasswordMessageToQueue(string userToName, string userToEmail, string userFromName, string userFromEmail, string resetPasswordUrl)
        //{
        //    var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);
        //    var message = new BrokeredMessage();
        //    message.Properties.Add("UserToName", userToName);
        //    message.Properties.Add("UserFromName", userFromName);
        //    message.Properties.Add("UserToEmail", userToEmail);
        //    message.Properties.Add("UserFromEmail", userFromEmail);
        //    message.Properties.Add("ResetPasswordUrl", resetPasswordUrl);
        //    message.Properties.Add("Subject", "Reset password request");
        //    message.Properties.Add("Type", "ResetPassword");
        //    client.Send(message);
        //}

        //public static void AddAssigneeChangedMessageToQueue(string userToName, string userToEmail, string userFromName, string userFromEmail, string bugTitle, string projectName)
        //{
        //    var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);
        //    var message = new BrokeredMessage();
        //    message.Properties.Add("UserToName", userToName);
        //    message.Properties.Add("UserFromName", userFromName);
        //    message.Properties.Add("UserToEmail", userToEmail);
        //    message.Properties.Add("UserFromEmail", userFromEmail);
        //    message.Properties.Add("BugTitle", bugTitle);
        //    message.Properties.Add("ProjectName", projectName);
        //    message.Properties.Add("Subject", "You was assigned to a bug");
        //    message.Properties.Add("Type", "AssigneeChanged");
        //    client.Send(message);
        //}

        //public static void AddBugStateChangedMessageToQueue(string userToName, string userToEmail, string userFromName, string userFromEmail, string bugTitle, string projectName)
        //{
        //    var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);
        //    var message = new BrokeredMessage();
        //    message.Properties.Add("UserToName", userToName);
        //    message.Properties.Add("UserFromName", userFromName);
        //    message.Properties.Add("UserToEmail", userToEmail);
        //    message.Properties.Add("UserFromEmail", userFromEmail);
        //    message.Properties.Add("BugTitle", bugTitle);
        //    message.Properties.Add("ProjectName", projectName);
        //    message.Properties.Add("Subject", "Bug state was changed");
        //    message.Properties.Add("Type", "BugStateChanged");
        //    client.Send(message);
        //}

        public static void AddResetPasswordMessageToQueue(string userToName, string userToEmail, string resetPasswordUrl)
        {
            var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);
            var message = new BrokeredMessage();
            message.Properties.Add("UserToName", userToName);
            message.Properties.Add("UserToEmail", userToEmail);
            message.Properties.Add("ResetPasswordUrl", resetPasswordUrl);
            message.Properties.Add("Subject", "Reset password request");
            message.Properties.Add("Type", "ResetPassword");
            client.Send(message);
        }

        public static void AddAssigneeChangedMessageToQueue(string userToName, string userToEmail, string bugTitle, string projectName)
        {
            var client = QueueClient.CreateFromConnectionString(ConnectionString, QueueName);
            var message = new BrokeredMessage();
            message.Properties.Add("UserToName", userToName);
            message.Properties.Add("UserToEmail", userToEmail);
            message.Properties.Add("BugTitle", bugTitle);
            message.Properties.Add("ProjectName", projectName);
            message.Properties.Add("Subject", "You was assigned to a bug");
            message.Properties.Add("Type", "AssigneeChanged");
            client.Send(message);
        }
    }
}
