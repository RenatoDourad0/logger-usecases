using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using AeC.digitalJourney.Lib.Monitor;

//comando para rodar
// set GOOGLE_APPLICATION_CREDENTIALS=C:\Users\renato.dourado\Documents\gcp_auth\pub-sub.json && dotnet run
class Program
{
    static async Task Main(string[] args)
    {
        var env = new Env { ENV = "production", LOG_MIN_LEVEL = "debug", OUTPUT = "file", APPLICATION="minimal-pubSub" };
        var logger = LoggerConfigurationHelper.ConfigureLogger(env);

        string projectId = "united-concord-400318";
        string topicId = "test";
        string subscriptionId = "test-sub";

        var topicName = new TopicName(projectId, topicId);
        var subscriptionName = new SubscriptionName(projectId, subscriptionId);

        var subscriber = SubscriberClient.Create(subscriptionName);

        await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancel) =>
        {
            await Task.Run(() => {
                var attributes = new Dictionary<string, string>(message.Attributes);

                GcpPubSubLoggingEnricher.Enrich(message, logger);

                logger.Information(message.Data.ToStringUtf8(), "200", attributes);
                logger.ClearContext();
            });
            return SubscriberClient.Reply.Ack;
        });
    }
}
