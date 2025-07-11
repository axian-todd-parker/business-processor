# Axian Business Processor: Project Context

This document provides a high-level overview of the Axian Business Processor project, including its structure and key architectural decisions.

## Project Overview

The `axian-todd-parker/business-processor` project is a collection of services and tools designed for processing business data. It follows a microservices-oriented architecture to ensure scalability and maintainability.

### Key Components

* **`BusinessProcessor`**: The core service of the project, written in F#. It is responsible for consuming messages from an AWS SQS queue and executing business logic based on the message content.
* **`CloudFormation`**: This directory contains Infrastructure as Code (IaC) templates for provisioning and managing the project's AWS resources, ensuring consistent and repeatable deployments.
* **`DataGenerator`**: A utility for generating synthetic data, crucial for testing the `BusinessProcessor` and other components under various scenarios.
* **`Docker`**: Contains Dockerfiles and related configurations for building container images of the applications, facilitating local development and deployment.
* **`LogAggregator`**: A service designed to collect, process, and store logs from all other services, providing a centralized point for monitoring and debugging.
* **`ResourceAutomation`**: Includes scripts and tools for automating various operational tasks, such as resource cleanup or scheduled jobs.
* **`scripts`**: A general-purpose directory for utility scripts that support development and operational workflows.

