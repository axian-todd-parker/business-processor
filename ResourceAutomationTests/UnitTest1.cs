using System.Text.Json;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using Moq;
using Xunit;

public class ResourceAutomationFunctionTests
{
    private readonly Mock<IAmazonEC2> _mockEc2 = new();
    private readonly Function _function;

    public ResourceAutomationFunctionTests()
    {
        _function = new Function(_mockEc2.Object);
    }

    [Fact]
    public async Task CallsStartInstance_WhenActionIsStartInstance()
    {
        var command = new AutomationCommand
        {
            Action = "start-instance",
            ResourceId = "i-1234567890abcdef"
        };

        var evnt = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new SQSEvent.SQSMessage
                {
                    Body = JsonSerializer.Serialize(command)
                }
            }
        };

        await _function.FunctionHandler(evnt, new TestLambdaContext());

        _mockEc2.Verify(e => e.StartInstancesAsync(
            It.Is<StartInstancesRequest>(req =>
                req.InstanceIds.Contains("i-1234567890abcdef")),
            default), Times.Once);
    }

    // Additional tests: stop-instance, modify-instance-type, invalid input, etc.
}
