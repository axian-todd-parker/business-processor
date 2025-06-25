// ResourceAutomationFunction.cs
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.EC2;
using Amazon.EC2.Model;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

public class Function
{
    private readonly AmazonEC2Client _ec2Client = new();

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        foreach (var message in evnt.Records)
        {
            var command = JsonSerializer.Deserialize<AutomationCommand>(message.Body);
            if (command == null) continue;

            switch (command.Action.ToLower())
            {
                case "start-instance":
                    await _ec2Client.StartInstancesAsync(new StartInstancesRequest
                    {
                        InstanceIds = new List<string> { command.ResourceId }
                    });
                    break;

                case "stop-instance":
                    await _ec2Client.StopInstancesAsync(new StopInstancesRequest
                    {
                        InstanceIds = new List<string> { command.ResourceId }
                    });
                    break;

                case "modify-instance-type":
                    await _ec2Client.StopInstancesAsync(new StopInstancesRequest
                    {
                        InstanceIds = new List<string> { command.ResourceId }
                    });

                    await _ec2Client.ModifyInstanceAttributeAsync(new ModifyInstanceAttributeRequest
                    {
                        InstanceId = command.ResourceId,
                        InstanceType = command.Parameter
                    });

                    await _ec2Client.StartInstancesAsync(new StartInstancesRequest
                    {
                        InstanceIds = new List<string> { command.ResourceId }
                    });
                    break;

                default:
                    context.Logger.LogWarning($"Unknown action: {command.Action}");
                    break;
            }
        }
    }
}

public class AutomationCommand
{
    public string Action { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty; // optional for some actions
}
