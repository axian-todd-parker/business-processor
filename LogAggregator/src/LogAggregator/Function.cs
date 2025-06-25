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
        using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        using SqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = BuildInsertQuery();
        AddSqlParameters(cmd, log);

        try
        {
            int result = await cmd.ExecuteNonQueryAsync();
            if (result != 1)
            {
                Console.WriteLine($"Unexpected insert result: {result}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing log to DB: {ex.Message}");
        }
    }

    private static string BuildInsertQuery()
    {
        return "INSERT INTO Logs (Timestamp, Level, Message) VALUES (@ts, @lvl, @msg)";
    }

    private static void AddSqlParameters(SqlCommand cmd, LogMessage log)
    {
        cmd.Parameters.AddWithValue("@ts", log.Timestamp);
        cmd.Parameters.AddWithValue("@lvl", log.Level);
        cmd.Parameters.AddWithValue("@msg", log.Message);
    }

}

public class LogMessage
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "Info";
    public string Message { get; set; } = string.Empty;
}