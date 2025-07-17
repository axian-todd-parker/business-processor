Hello!

You've found the business-processor. This project processes business processes. It does so in a businessy way.

This project uses a micro-service architecture to generate messages for one of three queues. 
The first queue is for log messages, and saves them to a database via a lambda function.
The second queue is for business processing. Using a docker container to run a process that consumes messages, then runs a business process on those messages.
The third queue is another lambda function that consumes messages and updates AWS resources accordingly.

This code does nothing useful. It was written (almost) entirely by AI. It is intended to illustrate the use of AI for code-generation. I have never run this project in a live environment.
