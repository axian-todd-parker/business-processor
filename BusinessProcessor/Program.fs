// BusinessProcessor.fsx - F#
open System
open System.Text.Json
open Amazon.SQS
open Amazon.SQS.Model
open System.Threading.Tasks

// Define the message type
type BusinessMessage = {
    CustomerId: string
    ActionType: string
    Amount: float
    Timestamp: DateTime
}

// Business logic processor
let processBusinessLogic (msg: BusinessMessage) =
    printfn "[BusinessLogic] Processing: %s — %s on %O" msg.CustomerId msg.ActionType msg.Timestamp
    if msg.ActionType = "purchase" then
        printfn "Analyzing purchase pattern..."
        if msg.Amount > 100.0 then
            printfn "[INSIGHT] High-value customer!"

[<EntryPoint>]
let main _ =
    let queueUrl =
        match Environment.GetEnvironmentVariable("QUEUE_URL") with
        | null -> failwith "QUEUE_URL not set"
        | url -> url

    use sqsClient = new AmazonSQSClient()
    let idleDelay = TimeSpan.FromMinutes(2.0)

    let rec pollLoop () =
        task {
            let! response =
                sqsClient.ReceiveMessageAsync(ReceiveMessageRequest(QueueUrl = queueUrl, MaxNumberOfMessages = 10, WaitTimeSeconds = 1))

            if response.Messages.Count = 0 then
                printfn "No messages. Sleeping..."
                do! Task.Delay idleDelay
            else
                for msg in response.Messages do
                    try
                        match JsonSerializer.Deserialize<BusinessMessage>(msg.Body) with
                        | data ->
                            processBusinessLogic data
                            let! _ = sqsClient.DeleteMessageAsync(queueUrl, msg.ReceiptHandle)
                            ()
                    with ex ->
                        printfn "[ERROR] Failed to process message: %s" ex.Message

            return! pollLoop ()
        }

    pollLoop().GetAwaiter().GetResult()
    0
