// Program.cs - DataGenerator using top-level statements
using Amazon;
using Amazon.SQS;
using System.Text.Json;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var regionConfig = config.GetSection("AWS:Region").Get<string>();
if (string.IsNullOrEmpty(regionConfig))
{
    throw new ArgumentNullException("AWS:Region");
}
var region = RegionEndpoint.GetBySystemName(regionConfig);
var sns = new AmazonSQSClient(region);
var queueUrls = config.GetSection("AWS:QueueUrls").Get<Dictionary<string, string>>();
if (queueUrls == null || queueUrls.Count() != 3)
{
    throw new ArgumentNullException("AWS:QueueUrls");
}
var random = new Random();

while (true)
{
    var type = new[] { "Alpha", "Beta", "Gamma" }[random.Next(3)];
    var message = new
    {
        Timestamp = DateTime.UtcNow,
        Level = random.Next(3) == 0 ? "Error" : "Info",
        ActionType = random.Next(2) == 0 ? "purchase" : "view",
        Amount = random.NextDouble() * 200,
        CustomerId = Guid.NewGuid().ToString(),
        ResourceId = "i-1234567890abcdef0"
    };

    var json = JsonSerializer.Serialize(message);
    await sns.SendMessageAsync(queueUrls[type], json);
    Console.WriteLine($"Published to {type}: {json}");
    await Task.Delay(2000);
}
