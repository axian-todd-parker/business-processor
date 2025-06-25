using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SimpleNotificationService;
using System.Text.Json;
using Microsoft.Data.SqlClient;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

public class Function
{
    private static readonly string _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")!;
    private static readonly string _snsTopicArn = Environment.GetEnvironmentVariable("SNS_TOPIC_ERROR")!;
    private readonly AmazonSimpleNotificationServiceClient _snsClient = new();

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        foreach (var message in evnt.Records)
        {
            var log = JsonSerializer.Deserialize<LogMessage>(message.Body);
            if (log == null) continue;

            await SaveToDatabase(log);

            if (log.Level == "Error")
            {
                await _snsClient.PublishAsync(_snsTopicArn, $"[Error] {log.Message}", "LogAggregator Alert");
            }
        }
    }

    private async Task SaveToDatabase(LogMessage log)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Logs (Timestamp, Level, Message) VALUES (@ts, @lvl, @msg)";
        cmd.Parameters.AddWithValue("@ts", log.Timestamp);
        cmd.Parameters.AddWithValue("@lvl", log.Level);
        cmd.Parameters.AddWithValue("@msg", log.Message);
        await cmd.ExecuteNonQueryAsync();
    }
}

public class LogMessage
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "Info";
    public string Message { get; set; } = string.Empty;
}