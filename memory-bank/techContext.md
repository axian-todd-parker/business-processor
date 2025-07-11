# Technology Context

This document lists the core technologies, frameworks, and services used in the Axian Business Processor project.

## 1. Programming Languages

- **F#**: The primary language for the `BusinessProcessor` service, chosen for its strong type system, immutability by default, and functional programming paradigm, which are well-suited for building robust and maintainable business logic.

## 2. Cloud Services (AWS)

- **AWS Simple Queue Service (SQS)**: Used as the message broker to decouple services and manage asynchronous communication.
- **AWS CloudFormation**: The Infrastructure as Code (IaC) tool for provisioning and managing all AWS resources.
- **Other AWS Services**: The project may also leverage other services like S3 for storage, IAM for security, and CloudWatch for monitoring, although SQS is the central component.

## 3. Tools and Frameworks

- **.NET Core**: The runtime environment for the F# `BusinessProcessor` service.
- **Docker**: Used for containerizing applications to ensure consistency across development, testing, and production environments.

## 4. Development and Operations

- **Git**: The version control system for managing the codebase.
- **Markdown**: Used for all project documentation, including this file.
