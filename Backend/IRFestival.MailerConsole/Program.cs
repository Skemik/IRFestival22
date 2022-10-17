// See https://aka.ms/new-console-template for more information

using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using IRFestival.Api.Domain;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var connectionString= "Endpoint=sb://irfestivalservicebussk.servicebus.windows.net/;SharedAccessKeyName=listener;SharedAccessKey=i0HNrc9tlCHX3NK2nNUSdAK9DyyyxpA7NK4/fpMJI5U=;EntityPath=mails";
var queueName="mails";
await using (var client = new ServiceBusClient(connectionString))
{
    var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;
    await processor.StartProcessingAsync();
    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("\n Stopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}


static async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Mail test = JsonConvert.DeserializeObject<Mail>(body);
    Console.WriteLine($"Mail to send : {test.Message}\n Receiver : {test.EmailAddress}");

    await args.CompleteMessageAsync(args.Message);
}

static  Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}